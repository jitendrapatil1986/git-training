namespace Warranty.Server.Handlers.ServiceCalls
{
    using Core.Entities;
    using Events;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyServiceCallEmployeeReassignedHandler : IHandleMessages<InnerMessages.NotifyServiceCallEmployeeReassigned>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;

        public NotifyServiceCallEmployeeReassignedHandler(IBus bus, IDatabase database)
        {
            _bus = bus;
            _database = database;
        }

        public void Handle(NotifyServiceCallEmployeeReassigned message)
        {
            using (_database)
            {
                var serviceCall = _database.SingleById<ServiceCall>(message.ServiceCallId);
                var employee = _database.SingleById<Employee>(serviceCall.WarrantyRepresentativeEmployeeId);

                _bus.Publish<ServiceCallEmployeeReassigned>(x =>
                {
                    x.ServiceCallNumber = serviceCall.ServiceCallNumber;
                    x.EmployeeName = employee.Name;
                    x.EmployeeNumber = employee.Number;
                });
            }
        }
    }
}