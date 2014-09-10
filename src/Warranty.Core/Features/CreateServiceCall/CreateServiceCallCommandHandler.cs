using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warranty.Core.Features.CreateServiceCall
{
    using Entities;
    using Enumerations;
    using NPoco;
    using Security;

    public class CreateServiceCallCommandHandler: ICommandHandler<CreateServiceCallModel, Guid>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public CreateServiceCallCommandHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public Guid Handle(CreateServiceCallModel message)
        {
            var user = _userSession.GetCurrentUser();
            
            using (_database)
            {
                const string sql = @"SELECT TOP 1 ServiceCallNumber
                                    FROM ServiceCalls
                                    ORDER BY ServiceCallNumber DESC";

                var lastCall = _database.First<int>(sql);
                var newCallNumber = lastCall + 1;

                //TODO: change logic to generate call number

                var employeeId = GetEmployeeIdForWsc(message);

                var serviceCall = new ServiceCall
                {
                    ServiceCallNumber = newCallNumber,
                    ServiceCallStatus = ServiceCallStatus.Requested, //TODO: Set to requested for now until we figure out who is WSR and WC. ServiceCallStatus.Open,
                    JobId = message.JobId,
                    Contact = message.Contact,
                    WarrantyRepresentativeEmployeeId = employeeId,
                    ServiceCallType = "Warranty Service Request",
                    CompletionDate = null,
                };

                _database.Insert(serviceCall);

                if (message.ServiceCallLineItems != null)
                {
                    foreach (var line in message.ServiceCallLineItems)
                    {
                        var serviceCallLine = new ServiceCallLineItem
                            {
                                ServiceCallId = serviceCall.ServiceCallId,
                                ServiceCallLineItemId = Guid.NewGuid(),
                                LineNumber = line.LineItemNumber,
                                ProblemCode = line.ProblemCodeDisplayName,
                                ProblemDescription = line.ProblemDescription,
                            };

                        _database.Insert(serviceCallLine);
                    }
                }

                //TODO: Remove if not saving header notes.
                if (message.ServiceCallHeaderComments != null)
                {
                    foreach (var noteLine in message.ServiceCallHeaderComments)
                    {
                        var serviceCallComment = new ServiceCallComment
                            {
                                ServiceCallId = serviceCall.ServiceCallId,
                                ServiceCallCommentId = Guid.NewGuid(),
                                Comment = noteLine.Comment,
                            };

                        _database.Insert(serviceCallComment);
                    }
                }

                return serviceCall.ServiceCallId;
            }
        }

        private Guid? GetEmployeeIdForWsc(CreateServiceCallModel message)
        {
            const string sqlEmployeeId = @"SELECT TOP 1 ca.EmployeeId
                                                    FROM CommunityAssignments ca
                                                    INNER JOIN Jobs j
                                                    ON ca.CommunityId = j.CommunityId
                                                    WHERE j.JobId = @0";


            var employeeId = _database.FirstOrDefault<Guid>(sqlEmployeeId, message.JobId);

            return employeeId == Guid.Empty ? (Guid?)null : employeeId;
        }
    }
}
