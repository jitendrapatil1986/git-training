namespace Warranty.Server.Handlers.Payments
{
    using Accounting.Events.Backcharge;
    using Core.Entities;
    using Core.Enumerations;
    using NPoco;
    using NServiceBus;

    public class RequestBackchargeResponseHandler : IHandleMessages<RequestBackchargeResponse>
    {
        private readonly IDatabase _database;

        public RequestBackchargeResponseHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(RequestBackchargeResponse message)
        {
            using (_database)
            {
                var backcharge = _database.SingleById<Backcharge>(message.BackchargeIdentifier);
                
                backcharge.BackchargeStatus = BackchargeStatus.Pending;
                backcharge.JdeIdentifier = message.JdeIdentifier;
                _database.Update(backcharge);
            }
        }
    }
}
