using System;
using Accounting.Events.Payment;
using NPoco;
using NServiceBus;
using Warranty.Core.Security;

namespace Warranty.Server.Handlers
{
    public class PaymentIdUpdatedHandler : IHandleMessages<PaymentIdUpdated>
    {
        private readonly IDatabase _database;
        private readonly IUser _user;

        public PaymentIdUpdatedHandler(IDatabase database, IUser user)
        {
            _database = database;
            _user = user;
        }

        public void Handle(PaymentIdUpdated message)
        {
            using (_database)
            {
                const string sql = @"UPDATE Payments
                                        SET JdeIdentifier = @0,
                                            UpdatedDate = @1,
                                            UpdatedBy = @2
                                        WHERE JdeIdentifier = @3";

                _database.Execute(sql, message.New_JDEId, DateTime.Now, _user.UserName, message.Old_JDEId);
            }
        }
    }
}