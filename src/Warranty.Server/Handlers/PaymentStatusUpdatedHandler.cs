using Accounting.Events.Payment;
using NPoco;
using NServiceBus;

namespace Warranty.Server.Handlers
{
    using Core.Entities;
    using Extensions;

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
