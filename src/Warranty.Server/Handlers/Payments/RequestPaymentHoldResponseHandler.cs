namespace Warranty.Server.Handlers.Payments
{
    using System;
    using Accounting.Events.Payment;
    using Core.Entities;
    using Core.Enumerations;
    using NPoco;
    using NServiceBus;

    public class RequestPaymentHoldResponseHandler : IHandleMessages<RequestPaymentHoldResponse>
    {
        private readonly IDatabase _database;

        public RequestPaymentHoldResponseHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(RequestPaymentHoldResponse message)
        {
            using (_database)
            {
                var payment = _database.SingleById<Payment>(new Guid(message.PaymentId));
                payment.PaymentStatus = PaymentStatus.Hold;
                _database.Update(payment);
            }
        }
    }
}


