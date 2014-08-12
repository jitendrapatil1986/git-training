using Accounting.Events.Payment;
using NPoco;
using NServiceBus;
using Warranty.Core.Entities;

namespace Warranty.Server.Handlers
{
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
                _database.Save<Payment>(new Payment
                {
                    VendorNumber = message.AddressNumber,
                    Amount = message.PaymentAmount,
                    PaymentStatus = message.Status,
                    JobNumber = message.CostCenter,
                    JdeIdentifier = message.JDEId,
                });
            }
        }
    }
}