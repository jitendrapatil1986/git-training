using Accounting.Events.Payment;
using NPoco;
using NServiceBus;
using Warranty.Core.Entities;

namespace Warranty.Server.Handlers
{
    public class PaymentAddressNumberUpdatedHandler : IHandleMessages<PaymentAddressNumberUpdated>
    {
        private readonly IDatabase _database;

        public PaymentAddressNumberUpdatedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(PaymentAddressNumberUpdated message)
        {
            using (_database)
            {
                const string sql = @"UPDATE Payments
                                        SET VendorNumber = {0}
                                        WHERE JdeIdentifier = {1}";

                _database.Execute(sql, message.AddressNumber, message.JDEId);
            }
        }
    }
}