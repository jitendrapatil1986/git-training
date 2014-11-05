namespace Warranty.Server.Handlers.Payments
{
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyBackchargeApprovedHandler : IHandleMessages<NotifyBackchargeApproved>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;

        public NotifyBackchargeApprovedHandler(IBus bus, IDatabase database)
        {
            _bus = bus;
            _database = database;
        }

        public void Handle(NotifyBackchargeApproved message)
        {
            throw new System.NotImplementedException();
        }
    }
}
