namespace Warranty.Server.Handlers.Payments
{
    using Core.Entities;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyPaymentDeletedHandler : IHandleMessages<NotifyPaymentDeleted>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;

        public NotifyPaymentDeletedHandler(IBus bus, IDatabase database)
        {
            _bus = bus;
            _database = database;
        }

        public void Handle(NotifyPaymentDeleted message)
        {
            using (_database)
            {
                var payment = _database.SingleById<Payment>(message.PaymentId);

                var command = new Accounting.Commands.Payments.RequestDeletePendingPayment
                    {
                        PaymentIdentifier = message.PaymentId.ToString(),
                        Username = message.UserName,
                        JdeIdentifier = payment.JdeIdentifier
                    };
                _bus.Send(command);
            }
        }
    }
}