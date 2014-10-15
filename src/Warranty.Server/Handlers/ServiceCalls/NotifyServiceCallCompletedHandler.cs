namespace Warranty.Server.Handlers.ServiceCalls
{
    using System;
    using Core.Entities;
    using Core.Enumerations;
    using Events;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyServiceCallCompletedHandler : IHandleMessages<InnerMessages.NotifyServiceCallCompleted>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;

        public NotifyServiceCallCompletedHandler(IBus bus, IDatabase database)
        {
            _bus = bus;
            _database = database;
        }

        public void Handle(InnerMessages.NotifyServiceCallCompleted message)
        {
            using (_database)
            {
                var serviceCall = _database.SingleById<ServiceCall>(message.ServiceCallId);
                _bus.Publish<ServiceCallCompleted>(x =>
                    {
                        x.ServiceCallNumber = serviceCall.ServiceCallNumber;
                        x.ServiceCallStatus = ServiceCallStatus.Complete.DisplayName;
                        x.CompletionDate = serviceCall.CompletionDate;
                    });
            }
        }
    }
}