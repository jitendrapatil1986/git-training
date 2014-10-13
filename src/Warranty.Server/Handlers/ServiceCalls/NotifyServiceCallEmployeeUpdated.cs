namespace Warranty.Server.Handlers.ServiceCalls
{
    using Core.Entities;
    using Events;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyServiceCallEmployeeUpdatedHandler : IHandleMessages<NotifyServiceCallEmployeeUpdated>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;

        public NotifyServiceCallEmployeeUpdatedHandler(IBus bus, IDatabase database)
        {
            _bus = bus;
            _database = database;
        }

        public void Handle(NotifyServiceCallEmployeeUpdated message)
        {
            using (_database)
            {
                var serviceCall = _database.SingleById<ServiceCall>(message.ServiceCallId);
                var employee = _database.SingleById<Employee>(serviceCall.WarrantyRepresentativeEmployeeId);

                _bus.Publish<ServiceCallEmployeeUpdated>(x =>
                {
                    x.ServiceCallNumber = serviceCall.ServiceCallNumber;
                    x.EmployeeName = employee.Name;
                    x.EmployeeNumber = employee.Number;
                });
            }
        }
    }
}