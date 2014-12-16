namespace Warranty.Server.Handlers.Payments
{
    using System;
    using Accounting.Events.Payment;
    using Core.Entities;
    using NPoco;
    using NServiceBus;

    public class RequestDeletePendingPaymentResponseHandler : IHandleMessages<RequestDeletePendingPaymentResponse>
    {
        private readonly IDatabase _database;

        public RequestDeletePendingPaymentResponseHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(RequestDeletePendingPaymentResponse message)
        {
            using (_database)
            {
                var payment = _database.SingleById<Payment>(new Guid(message.PaymentId));
                var backcharge = _database.Single<Backcharge>("Where PaymentId = @0", message.PaymentId);

                if (backcharge != null)
                {
                    _database.Delete(backcharge);
                }

                _database.Delete(payment);
            }
        }
    }
}