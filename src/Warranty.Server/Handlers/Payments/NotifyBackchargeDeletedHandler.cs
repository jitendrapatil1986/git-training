namespace Warranty.Server.Handlers.Payments
{
    using Core.Entities;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyBackchargeDeletedHandler : IHandleMessages<NotifyBackchargeDeleted>
    {
        private readonly IDatabase _database;
        private readonly IBus _bus;

        public NotifyBackchargeDeletedHandler(IDatabase database, IBus bus)
        {
            _database = database;
            _bus = bus;
        }

        public void Handle(NotifyBackchargeDeleted message)
        {
            using (_database)
            {
                var backcharge = _database.SingleById<Backcharge>(message.BackchargeId);

                var backchargeCommand = new Accounting.Commands.Backcharges.RequestDeleteBackcharge
                {
                    JdeIdentifier = backcharge.JdeIdentifier
                };
                _bus.Send(backchargeCommand);

                _database.Delete(backcharge);
            }
        }
    }
}