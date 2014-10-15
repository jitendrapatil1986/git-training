namespace Warranty.Server.Handlers.ServiceCalls
{
    using Core.Entities;
    using Events;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyServiceCallSpecialProjectStatusChangedHandler : IHandleMessages<NotifyServiceCallSpecialProjectStatusChanged>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;

        public NotifyServiceCallSpecialProjectStatusChangedHandler(IBus bus, IDatabase database)
        {
            _bus = bus;
            _database = database;
        }

        public void Handle(NotifyServiceCallSpecialProjectStatusChanged message)
        {
            using (_database)
            {
                var serviceCall = _database.SingleById<ServiceCall>(message.ServiceCallId);

                _bus.Publish<ServiceCallSpecialProjectStatusChanged>(x =>
                {
                    x.ServiceCallNumber = serviceCall.ServiceCallNumber;
                    x.SpecialProject = serviceCall.IsSpecialProject;
                    x.SpecialProjectReason = message.SpecialProjectReason;
                    x.SpecialProjectDate = message.SpecialProjectDate;
                });
            }
        }
    }
}