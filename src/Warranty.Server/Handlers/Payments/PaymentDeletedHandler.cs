using Accounting.Events.Payment;
using NPoco;
using NServiceBus;
using Warranty.Core.Entities;
using Warranty.Server.Extensions;

namespace Warranty.Server.Handlers.Payments
{
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
                var payment = _database.SingleOrDefaultByJdeId<Payment>(message.JDEId);
                if (payment != null)
                {
                    var backcharges = _database.Fetch<Backcharge>("WHERE PaymentId = @0", payment.PaymentId);
                    foreach (var backcharge in backcharges)
                    {
                        backcharge.PaymentId = null;
                        _database.Update(backcharge);
                    }

                    _database.Delete(payment);
                }
            }
        }
    }
}
