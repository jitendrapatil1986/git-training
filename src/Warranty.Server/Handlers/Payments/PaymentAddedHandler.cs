using Accounting.Events.Payment;
using NPoco;
using NServiceBus;
using Warranty.Core.Entities;
using Warranty.Server.Configuration;
using Warranty.Server.Extensions;

namespace Warranty.Server.Handlers.Payments
{
    using System;
    using Core.Enumerations;

    public class PaymentAddedHandler : IHandleMessages<PaymentAdded>
    {
        private readonly IDatabase _database;

        public PaymentAddedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(PaymentAdded message)
        {
            if (!WarrantyConstants.LaborObjectAccounts.Contains(message.ObjectAccount))
                return;

            using (_database)
            {
                if (_database.ExistsByJdeId<Payment>(message.JDEId))
                    return;

                _database.Insert(new Payment
                {
                    VendorNumber = message.AddressNumber,
                    Amount = message.PaymentAmount,
                    PaymentStatus = PaymentStatus.FromJdeCode(message.Status),
                    JobNumber = message.CostCenter,
                    JdeIdentifier = message.JDEId,
                });
            }
        }
    }

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
