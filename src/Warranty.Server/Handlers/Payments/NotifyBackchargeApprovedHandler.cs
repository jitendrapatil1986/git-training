namespace Warranty.Server.Handlers.Payments
{
    using System;
    using Configuration;
    using Core.Entities;
    using Core.Security;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyBackchargeApprovedHandler : IHandleMessages<NotifyBackchargeApproved>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public NotifyBackchargeApprovedHandler(IBus bus, IDatabase database, IUserSession userSession)
        {
            _bus = bus;
            _database = database;
            _userSession = userSession;
        }

        public void Handle(NotifyBackchargeApproved message)
        {
            using (_database)
            {
                
            }
        
        }
    }
}
