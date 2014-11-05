namespace Warranty.Server.Handlers.Payments
{
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyBackchargeOnHoldHandler : IHandleMessages<NotifyBackchargeOnHold>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;

        public NotifyBackchargeOnHoldHandler(IBus bus, IDatabase database)
        {
            _bus = bus;
            _database = database;
        }

        public void Handle(NotifyBackchargeOnHold message)
        {
            throw new System.NotImplementedException();
        }
    }
}
