namespace Warranty.Server.Handlers.ServiceCalls
{
    using Events;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyServiceCallDeletedHandler : IHandleMessages<NotifyServiceCallDeleted>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;

        public NotifyServiceCallDeletedHandler(IBus bus, IDatabase database)
        {
            _bus = bus;
            _database = database;
        }

        public void Handle(NotifyServiceCallDeleted message)
        {
            using (_database)
            {
                _bus.Publish<ServiceCallDeleted>(x =>
                    {
                        x.ServiceCallId = message.ServiceCallId;
                    });
            }
        }
    }
}