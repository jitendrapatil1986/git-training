using Accounting.Events.Payment;
using NPoco;
using NServiceBus;
using Warranty.Core.Entities;
using Warranty.Server.Extensions;

namespace Warranty.Server.Handlers.Payments
{
    public class PaymentStatusUpdatedHandler : IHandleMessages<PaymentStatusUpdated>
    {
        private readonly IDatabase _database;

        public PaymentStatusUpdatedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(PaymentStatusUpdated message)
        {
            using (_database)
            {
                var payment = _database.SingleOrDefaultByJdeId<Payment>(message.JDEId);
                if (payment == null)
                    return;

                payment.PaymentStatus = message.Status;
                _database.Update(payment);
            }
        }
    }
}
