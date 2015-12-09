using System;
using System.Collections.Generic;
using System.Linq;
using Common.Extensions;
using NPoco;
using NPoco.Expressions;
using NServiceBus;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;
using Warranty.InnerMessages;

namespace Warranty.Core.Features.AssignWSRs
{
    public class AssignWSRCommandHandler : ICommandHandler<AssignWSRCommand>
    {
        private readonly IDatabase _database;
        private readonly IBus _bus;

        public AssignWSRCommandHandler(IDatabase database, IBus bus)
        {
            _database = database;
            _bus = bus;
        }

        public List<Task> GetTasksForCommunity(string communityNumber)
        {
            const string sql = @"SELECT 
                                    T.TaskId,
                                    T.EmployeeId,
                                    T.ReferenceId,
                                    T.Description,
                                    T.IsComplete,
                                    T.TaskType,
                                    T.CreatedDate,
                                    T.CreatedBy,
                                    T.UpdatedDate,
                                    T.UpdatedBy,
                                    T.IsNoAction
                                FROM Tasks T 
                                    INNER JOIN Jobs J
                                        ON T.ReferenceId = J.JobId
                                    INNER JOIN Communities C 
                                        ON J.CommunityId = C.CommunityId
                                    INNER JOIN Employees E
                                        ON E.EmployeeId = T.EmployeeId
                                WHERE 
                                    C.CommunityNumber = {0}
                                    AND T.IsComplete = 0";
            return _database.Fetch<Task>(String.Format(sql, communityNumber));
        } 

        public void Handle(AssignWSRCommand cmd)
        {
            using (_database)
            {
                const string sql = @"SELECT * FROM CommunityAssignments WHERE CommunityId = @0";

                var communityAssignment =
                    _database.FirstOrDefault<CommunityAssignment>(sql, cmd.CommunityId);

                var communityNumber =
                    _database.Single<string>(@"SELECT CommunityNumber FROM Communities WHERE CommunityId = @0",
                        cmd.CommunityId);

                if (string.IsNullOrWhiteSpace(communityNumber))
                    throw new Exception(string.Format("No community was found with the Id of {0}.", cmd.CommunityId));

                var employeeNumber =
                    _database.Single<string>(@"SELECT EmployeeNumber FROM Employees WHERE EmployeeId = @0",
                        cmd.EmployeeId);

                if (string.IsNullOrWhiteSpace(employeeNumber))
                    throw new Exception(string.Format("No team member was found with the Id of {0}.", cmd.EmployeeId));

                if (communityAssignment == null)
                {
                    var newCommunityAssignment = new CommunityAssignment
                    {
                        CommunityId = cmd.CommunityId,
                        EmployeeId = cmd.EmployeeId,
                    };

                    _database.Insert(newCommunityAssignment);

                    _bus.Send<NotifyWarrantyRepresentativeAssignedToCommunity>(x =>
                    {
                        x.CommunityId = cmd.CommunityId;
                        x.CommunityNumber = communityNumber;
                        x.EmployeeId = cmd.EmployeeId;
                        x.EmployeeNumber = employeeNumber;
                    });
                }
                else
                {
                    communityAssignment.EmployeeId = cmd.EmployeeId;
                    var tasks = GetTasksForCommunity(communityNumber);

                    using (_database.Transaction)
                    {
                        _database.BeginTransaction();
                        _database.Update(communityAssignment);

                        foreach (var task in tasks.Where(x => x.TaskType.Value.In(TaskType.GetAll().Where(t => t.IsTransferable).Select(t => t.Value))))
                        {
                            task.EmployeeId = cmd.EmployeeId;
                            _database.Update(task);
                        }
                        _database.CompleteTransaction();
                    }

                    _bus.Send<NotifyCommunityWarrantyRepresentativeAssignmentChanged>(x =>
                    {
                        x.CommunityId = cmd.CommunityId;
                        x.CommunityNumber = communityNumber;
                        x.EmployeeId = cmd.EmployeeId;
                        x.EmployeeNumber = employeeNumber;
                    });
                }
            }
        }
    }
}