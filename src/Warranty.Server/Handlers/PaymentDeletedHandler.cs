using Accounting.Events.Payment;
using NPoco;
using NServiceBus;

namespace Warranty.Server.Handlers
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
                const string sql = @"DELETE FROM Payments
                                        WHERE JdeIdentifier = @0";

                _database.Execute(sql, message.JDEId);
            }
        }
    }
}