namespace Warranty.Server.Handlers.ServiceCalls
{
    using System;
    using Core.Entities;
    using Core.Enumerations;
    using Events;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyServiceCallCreatedHandler : IHandleMessages<NotifyServiceCallCreated>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;

        public NotifyServiceCallCreatedHandler(IBus bus, IDatabase database)
        {
            _bus = bus;
            _database = database;
        }

        public void Handle(NotifyServiceCallCreated message)
        {
            using (_database)
            {
                var serviceCall = _database.SingleById<ServiceCall>(message.ServiceCallId);
                Employee employee = null;
                if (serviceCall.WarrantyRepresentativeEmployeeId != null)
                {
                    employee = _database.SingleById<Employee>(serviceCall.WarrantyRepresentativeEmployeeId);
                }
                var job = _database.SingleById<Job>(serviceCall.JobId);
                _bus.Publish<ServiceCallCreated>(x =>
                {
                    x.ServiceCallNumber = serviceCall.ServiceCallNumber;
                    x.ServiceCallType = serviceCall.ServiceCallType;
                    x.WorkSummary = serviceCall.WorkSummary;
                    x.ServiceCallStatus = serviceCall.ServiceCallStatus.DisplayName;
                    x.Contact = serviceCall.Contact;
                    x.EmployeeName = employee != null ? employee.Name : null;
                    x.EmployeeNumber = employee != null ? employee.Number : null;
                    x.JobNumber = job.JobNumber;
                });
            }
        }
    }
}
