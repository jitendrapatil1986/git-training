namespace Warranty.Server.Handlers.Payments
{
    using Accounting.Events.Payment;
    using Core.Entities;
    using Core.Enumerations;
    using NPoco;
    using NServiceBus;

    public class RequestPaymentResponseHandler : IHandleMessages<RequestPaymentResponse>
    {
        private readonly IDatabase _database;

        public RequestPaymentResponseHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(RequestPaymentResponse message)
        {
            using (_database)
            {
                var payment = _database.SingleOrDefaultById<Payment>(message.PaymentIdentifier);
                if (payment != null)
                {
                    payment.PaymentStatus = PaymentStatus.Pending;
                    _database.Update(payment);
                }
            }
        }
    }
}