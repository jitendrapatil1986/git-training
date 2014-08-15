using Accounting.Events.Payment;
using NPoco;
using NServiceBus;

namespace Warranty.Server.Handlers
{
    using Core.Entities;
    using Extensions;

    public class PaymentAddressNumberUpdatedHandler : IHandleMessages<PaymentAddressNumberUpdated>
    {
        private readonly IDatabase _database;

        public PaymentAddressNumberUpdatedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(PaymentAddressNumberUpdated message)
        {
            using (_database)
            {
                var payment = _database.SingleOrDefaultByJdeId<Payment>(message.JDEId);
                if (payment == null)
                    return;

                payment.VendorNumber = message.AddressNumber;
                _database.Update(payment);
            }
        }
    }
}
