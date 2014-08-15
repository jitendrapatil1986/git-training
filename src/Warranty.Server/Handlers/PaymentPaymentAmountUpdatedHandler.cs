using Accounting.Events.Payment;
using NPoco;
using NServiceBus;

namespace Warranty.Server.Handlers
{
    using Core.Entities;
    using Extensions;

    public class PaymentPaymentAmountUpdatedHandler : IHandleMessages<PaymentPaymentAmountUpdated>
    {
        private readonly IDatabase _database;

        public PaymentPaymentAmountUpdatedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(PaymentPaymentAmountUpdated message)
        {
            using (_database)
            {
                var payment = _database.SingleOrDefaultByJdeId<Payment>(message.JDEId);
                if (payment == null)
                    return;

                payment.Amount = message.PaymentAmount;
                _database.Update(payment);
            }
        }
    }
}
