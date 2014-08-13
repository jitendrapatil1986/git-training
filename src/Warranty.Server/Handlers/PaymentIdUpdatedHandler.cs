using Accounting.Events.Payment;
using NPoco;
using NServiceBus;

namespace Warranty.Server.Handlers
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
                const string sql = @"UPDATE Payments
                                        SET JdeIdentifier = @0
                                        WHERE JdeIdentifier = @1";

                _database.Execute(sql, message.New_JDEId, message.Old_JDEId);
            }
        }
    }
}