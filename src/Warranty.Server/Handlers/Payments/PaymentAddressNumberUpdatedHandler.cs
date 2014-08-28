using Accounting.Events.Payment;
using NPoco;
using NServiceBus;
using Warranty.Core.Entities;
using Warranty.Server.Extensions;

namespace Warranty.Server.Handlers.Payments
{
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
