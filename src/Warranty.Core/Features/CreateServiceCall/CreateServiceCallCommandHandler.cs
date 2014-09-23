using System;

namespace Warranty.Core.Features.CreateServiceCall
{
    using Entities;
    using Enumerations;
    using NPoco;
    using Security;

    public class CreateServiceCallCommandHandler: ICommandHandler<CreateServiceCallCommand, Guid>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public CreateServiceCallCommandHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public Guid Handle(CreateServiceCallCommand message)
        {
            var user = _userSession.GetCurrentUser();

            var status = ServiceCallStatus.Requested;
            if (user.IsInRole(UserRoles.WarrantyServiceCoordinator) || user.IsInRole(UserRoles.WarrantyServiceManager))
            {
                status = ServiceCallStatus.Open;
            }

            using (_database)
            {
                //TODO: change logic to generate call number
                const string sql = @"SELECT TOP 1 ServiceCallNumber
                                    FROM ServiceCalls
                                    ORDER BY ServiceCallNumber DESC";

                var lastCall = _database.First<int>(sql);
                var newCallNumber = lastCall + 1;

                var employeeId = GetEmployeeIdForWsc(message);

                var serviceCall = new ServiceCall
                {
                    ServiceCallNumber = newCallNumber,
                    ServiceCallStatus = status,
                    JobId = message.JobId,
                    WarrantyRepresentativeEmployeeId = employeeId,
                    ServiceCallType = RequestType.WarrantyRequest.DisplayName,
                };

                _database.Insert(serviceCall);

                if (message.ServiceCallLineItems != null)
                {
                    foreach (var line in message.ServiceCallLineItems)
                    {
                        var serviceCallLine = new ServiceCallLineItem
                            {
                                ServiceCallId = serviceCall.ServiceCallId,
                                LineNumber = line.LineNumber,
                                ProblemCode = line.ProblemCode,
                                ProblemDescription = line.ProblemDescription,
                                ServiceCallLineItemStatus = ServiceCallLineItemStatus.Open,
                            };

                        _database.Insert(serviceCallLine);
                    }
                }

                return serviceCall.ServiceCallId;
            }
        }

        private Guid? GetEmployeeIdForWsc(CreateServiceCallCommand message)
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
