using Accounting.Events.Payment;
using NPoco;
using NServiceBus;
using Warranty.Core.Entities;
using Warranty.Server.Extensions;

namespace Warranty.Server.Handlers.Payments
{
    public class PaymentIdUpdatedHandler : IHandleMessages<PaymentIdUpdated>
    {
        private readonly IDatabase _database;

        public PaymentIdUpdatedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(PaymentIdUpdated message)
        {
            using (_database)
            {
                var payment = _database.SingleOrDefaultByJdeId<Payment>(message.Old_JDEId);
                if (payment == null)
                    return;

                payment.JdeIdentifier = message.New_JDEId;
                _database.Update(payment);
            }
        }
    }
}
