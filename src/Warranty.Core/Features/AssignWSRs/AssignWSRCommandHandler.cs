using System;
using NPoco;
using NServiceBus;
using Warranty.Core.Entities;
using Warranty.Events;
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
                    _database.Update(communityAssignment);
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