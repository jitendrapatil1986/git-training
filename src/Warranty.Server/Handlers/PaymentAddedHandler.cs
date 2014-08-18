namespace Warranty.Server.Handlers
{
    using Accounting.Events.Payment;
    using Configuration;
    using Core.Entities;
    using Extensions;
    using NPoco;
    using NServiceBus;

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
                    PaymentStatus = message.Status,
                    JobNumber = message.CostCenter,
                    JdeIdentifier = message.JDEId,
                });
            }
        }
    }
}
