using Accounting.Events.Payment;
using NPoco;
using NServiceBus;
using Warranty.Core.Entities;
using Warranty.Server.Extensions;

namespace Warranty.Server.Handlers.Payments
{
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
}
