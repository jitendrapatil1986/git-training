namespace Warranty.Server.Handlers.Payments
{
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyPaymentOnHoldHandler : IHandleMessages<NotifyPaymentOnHold>
    {

        private readonly IBus _bus;
        private readonly IDatabase _database;

        public NotifyPaymentOnHoldHandler(IBus bus, IDatabase database)
        {
            _bus = bus;
            _database = database;
        }

        public void Handle(NotifyPaymentOnHold message)
        {
            throw new System.NotImplementedException();
        }
    }
}
