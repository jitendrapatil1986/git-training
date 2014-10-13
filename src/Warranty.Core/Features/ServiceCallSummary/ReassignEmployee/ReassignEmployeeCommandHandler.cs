namespace Warranty.Core.Features.ServiceCallSummary.ReassignEmployee
{
    using System;
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class ReassignEmployeeCommandHandler : ICommandHandler<ReassignEmployeeCommand>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _logger;
        private readonly IBus _bus;

        public ReassignEmployeeCommandHandler(IDatabase database, IActivityLogger logger, IBus bus)
        {
            _database = database;
            _logger = logger;
            _bus = bus;
        }

        public void Handle(ReassignEmployeeCommand message)
        {
            using (_database)
            {
                var serviceCall = _database.SingleById<ServiceCall>(message.Pk);

                var employeeModel = _database.SingleOrDefault<EmployeeModel>("SELECT EmployeeId, EmployeeName FROM Employees WHERE EmployeeNumber=@0", message.Value);

                serviceCall.WarrantyRepresentativeEmployeeId = employeeModel.EmployeeId;
                _database.Update(serviceCall);

                _bus.Send<NotifyServiceCallEmployeeUpdated>(x =>
                    {
                        x.ServiceCallId = serviceCall.ServiceCallId;
                    });
                
                var logDetails = string.Format("Employee {0} has been assigned to service call #{1}", employeeModel.DisplayName, serviceCall.ServiceCallNumber);
                _logger.Write("Reassign employee to service call", logDetails, serviceCall.ServiceCallId, ActivityType.Reassignment, ReferenceType.ServiceCall);
            }
        }

        public class EmployeeModel
        {
            public Guid EmployeeId { get; set; }
            public string DisplayName { get; set; }
        }
    }
}