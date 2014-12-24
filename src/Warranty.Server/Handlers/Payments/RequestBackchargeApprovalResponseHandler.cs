namespace Warranty.Server.Handlers.Payments
{
    using System;
    using Accounting.Events.Backcharge;
    using Core.Entities;
    using Core.Enumerations;
    using NPoco;
    using NServiceBus;

    public class RequestBackchargeApprovalResponseHandler : IHandleMessages<RequestBackchargeApprovalResponse>
    {
        private readonly IDatabase _database;

        public RequestBackchargeApprovalResponseHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(RequestBackchargeApprovalResponse message)
        {
            using (_database)
            {
                var backcharge = _database.SingleById<Backcharge>(new Guid(message.BackchargeId));
                backcharge.BackchargeStatus = BackchargeStatus.Approved;
                _database.Update(backcharge);
            }
        }
    }
}