namespace Warranty.Server.Handlers.Payments
{
    using System;
    using Accounting.Events.Backcharge;
    using Core.Entities;
    using Core.Enumerations;
    using NPoco;
    using NServiceBus;

    public class RequestBackchargeHoldResponseHandler : IHandleMessages<RequestBackchargeHoldResponse>
    {
        private readonly IDatabase _database;

        public RequestBackchargeHoldResponseHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(RequestBackchargeHoldResponse message)
        {
            using (_database)
            {
                var backcharge = _database.SingleById<Backcharge>(new Guid(message.BackchargeId));
                backcharge.BackchargeStatus = BackchargeStatus.Hold;
                _database.Update(backcharge);
            }
        }
    }
}