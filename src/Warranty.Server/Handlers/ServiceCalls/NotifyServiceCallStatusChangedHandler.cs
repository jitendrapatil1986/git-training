namespace Warranty.Server.Handlers.ServiceCalls
{
    using Core.Entities;
    using Events;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyServiceCallStatusChangedHandler : IHandleMessages<NotifyServiceCallStatusChanged>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;

        public NotifyServiceCallStatusChangedHandler(IBus bus, IDatabase database)
        {
            _bus = bus;
            _database = database;
        }

        public void Handle(NotifyServiceCallStatusChanged message)
        {
            using (_database)
            {
                var serviceCall = _database.SingleById<ServiceCall>(message.ServiceCallId);

                _bus.Publish<ServiceCallStatusChanged>(x =>
                    {
                        x.ServiceCallId = serviceCall.ServiceCallId;
                        x.StatusDisplayName = serviceCall.ServiceCallStatus.DisplayName;
                    });
            }
        }
    }
}