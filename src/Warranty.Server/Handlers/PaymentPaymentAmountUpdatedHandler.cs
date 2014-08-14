using System;
using Accounting.Events.Payment;
using NPoco;
using NServiceBus;
using Warranty.Core.Security;

namespace Warranty.Server.Handlers
{
    public class PaymentPaymentAmountUpdatedHandler : IHandleMessages<PaymentPaymentAmountUpdated>
    {
        private readonly IDatabase _database;
        private readonly IUser _user;

        public PaymentPaymentAmountUpdatedHandler(IDatabase database, IUser user)
        {
            _database = database;
            _user = user;
        }

        public void Handle(PaymentPaymentAmountUpdated message)
        {
            using (_database)
            {
                const string sql = @"UPDATE Payments
                                        SET Amount = @0,
                                            UpdatedDate = @1,
                                            UpdatedBy = @2
                                        WHERE JdeIdentifier = @3";

                _database.Execute(sql, message.PaymentAmount, DateTime.UtcNow, _user.UserName, message.JDEId);
            }
        }
    }
}