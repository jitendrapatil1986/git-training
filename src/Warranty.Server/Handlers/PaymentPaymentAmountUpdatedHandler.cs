using Accounting.Events.Payment;
using NPoco;
using NServiceBus;

namespace Warranty.Server.Handlers
{
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
                const string sql = @"UPDATE Payments
                                        SET Amount = {0}
                                        WHERE JdeIdentifier = {1}";

                _database.Execute(sql, message.PaymentAmount, message.JDEId);
            }
        }
    }
}