namespace Warranty.Server.Handlers.Payments
{
    using Core.Entities;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyPaymentApprovedHandler : IHandleMessages<NotifyPaymentApproved>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;

        public NotifyPaymentApprovedHandler(IBus bus, IDatabase database)
        {
            _bus = bus;
            _database = database;
        }

        public void Handle(NotifyPaymentApproved message)
        {
            using (_database)
            {
                var payment = _database.SingleById<Payment>(message.PaymentId);

                
            }
        }
    }
}
