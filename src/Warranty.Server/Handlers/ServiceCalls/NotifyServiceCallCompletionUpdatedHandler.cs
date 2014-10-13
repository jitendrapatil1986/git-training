namespace Warranty.Server.Handlers.ServiceCalls
{
    using System;
    using Core.Entities;
    using Core.Enumerations;
    using Events;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyServiceCallCompletionUpdatedHandler : IHandleMessages<NotifyServiceCallCompletionUpdated>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;

        public NotifyServiceCallCompletionUpdatedHandler(IBus bus, IDatabase database)
        {
            _bus = bus;
            _database = database;
        }

        public void Handle(NotifyServiceCallCompletionUpdated message)
        {
            using (_database)
            {
                var serviceCall = _database.SingleById<ServiceCall>(message.ServiceCallId);
                _bus.Publish<ServiceCallCompletionUpdated>(x =>
                    {
                        x.ServiceCallNumber = serviceCall.ServiceCallNumber;
                        x.ServiceCallStatus = ServiceCallStatus.Complete.DisplayName;
                        x.CompletionDate = serviceCall.CompletionDate;
                    });
            }
        }
    }
}