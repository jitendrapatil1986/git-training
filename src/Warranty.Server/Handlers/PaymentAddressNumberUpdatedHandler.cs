using System;
using Accounting.Events.Payment;
using NPoco;
using NServiceBus;
using Warranty.Core.Security;

namespace Warranty.Server.Handlers
{
    public class PaymentAddressNumberUpdatedHandler : IHandleMessages<PaymentAddressNumberUpdated>
    {
        private readonly IDatabase _database;
        private readonly IUser _user;

        public PaymentAddressNumberUpdatedHandler(IDatabase database, IUser user)
        {
            _database = database;
            _user = user;
        }

        public void Handle(PaymentAddressNumberUpdated message)
        {
            using (_database)
            {
                const string sql = @"UPDATE Payments
                                        SET VendorNumber = @0,
                                            UpdatedDate = @1,
                                            UpdatedBy = @2
                                        WHERE JdeIdentifier = @3";

                _database.Execute(sql, message.AddressNumber, DateTime.UtcNow, _user.UserName, message.JDEId);
            }
        }
    }
}