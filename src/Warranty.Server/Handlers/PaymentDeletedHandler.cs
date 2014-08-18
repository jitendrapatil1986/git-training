using Accounting.Events.Payment;
using NPoco;
using NServiceBus;

namespace Warranty.Server.Handlers
{
    using Core.Entities;
    using Extensions;

    public class PaymentDeletedHandler : IHandleMessages<PaymentDeleted>
    {
        private readonly IDatabase _database;

        public PaymentDeletedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(PaymentDeleted message)
        {
            using (_database)
            {
                var payment = _database.SingleByJdeId<Payment>(message.JDEId);
                if (payment != null)
                    _database.Delete(payment);
            }
        }
    }
}
