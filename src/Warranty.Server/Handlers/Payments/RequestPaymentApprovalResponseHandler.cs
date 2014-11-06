namespace Warranty.Server.Handlers.Payments
{
    using System;
    using Accounting.Events.Payment;
    using Core.Entities;
    using Core.Enumerations;
    using NPoco;
    using NServiceBus;

    public class RequestPaymentApprovalResponseHandler : IHandleMessages<RequestPaymentApprovalResponse>
    {
        private readonly IDatabase _database;

        public RequestPaymentApprovalResponseHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(RequestPaymentApprovalResponse message)
        {
            using (_database)
            {
                var payment = _database.SingleById<Payment>(new Guid(message.PaymentId));
                payment.PaymentStatus = PaymentStatus.Approved;
                _database.Update(payment);
            }
        }
    }
}