using Accounting.Events.Payment;
using NPoco;
using NServiceBus;

namespace Warranty.Server.Handlers
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
                const string sql = @"UPDATE Payments
                                        SET PaymentStatus = @0
                                        WHERE JdeIdentifier = @1";

                _database.Execute(sql, message.Status, message.JDEId);
            }
        }
    }
}