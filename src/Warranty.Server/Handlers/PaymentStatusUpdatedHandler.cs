using System;
using Accounting.Events.Payment;
using NPoco;
using NServiceBus;
using Warranty.Core.Security;

namespace Warranty.Server.Handlers
{
    public class PaymentStatusUpdatedHandler : IHandleMessages<PaymentStatusUpdated>
    {
        private readonly IDatabase _database;
        private readonly IUser _user;

        public PaymentStatusUpdatedHandler(IDatabase database, IUser user)
        {
            _database = database;
            _user = user;
        }

        public void Handle(PaymentStatusUpdated message)
        {
            using (_database)
            {
                const string sql = @"UPDATE Payments
                                        SET PaymentStatus = @0,
                                            UpdatedDate = @1,
                                            UpdatedBy = @2
                                        WHERE JdeIdentifier = @3";

                _database.Execute(sql, message.Status, DateTime.Now, _user.UserName, message.JDEId);
            }
        }
    }
}