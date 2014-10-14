namespace Warranty.Server.Handlers.ServiceCalls
{
    using System;
    using Core.Entities;
    using Events;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyServiceCallEscalatedStatusChangedHandler : IHandleMessages<NotifyServiceCallEscalatedStatusChanged>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;

        public NotifyServiceCallEscalatedStatusChangedHandler(IBus bus, IDatabase database)
        {
            _bus = bus;
            _database = database;
        }

        public void Handle(NotifyServiceCallEscalatedStatusChanged message)
        {
            using (_database)
            {
                var serviceCall = _database.SingleById<ServiceCall>(message.ServiceCallId);

                _bus.Publish<ServiceCallEscalatedStatusChanged>(x =>
                {
                    x.ServiceCallNumber = serviceCall.ServiceCallNumber;
                    x.Escalated = serviceCall.IsEscalated;
                    x.EscalationDate = message.EscalatedDate;
                    x.EscalationReason = message.EscalatedReason;
                });
            }
        }
    }
    
}