namespace Warranty.Server.Handlers.Payments
{
    using System;
    using Accounting.Events.Backcharge;
    using Core.Entities;
    using Core.Enumerations;
    using NPoco;
    using NServiceBus;

    public class RequestBackchargeDenialResponseHandler : IHandleMessages<RequestBackchargeDenialResponse>
    {
        private readonly IDatabase _database;

        public RequestBackchargeDenialResponseHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(RequestBackchargeDenialResponse message)
        {
            using (_database)
            {
                var backcharge = _database.SingleById<Backcharge>(new Guid(message.BackchargeId));
                backcharge.BackchargeStatus = BackchargeStatus.Denied;
                _database.Update(backcharge);
            }
        }
    }
}