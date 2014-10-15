namespace Warranty.Server.Handlers.ServiceCalls
{
    using Core.Entities;
    using Events;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyServiceCallLineItemCompletedHandler : IHandleMessages<NotifyServiceCallLineItemCompleted>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;

        public NotifyServiceCallLineItemCompletedHandler(IBus bus, IDatabase database)
        {
            _bus = bus;
            _database = database;
        }

        public void Handle(NotifyServiceCallLineItemCompleted message)
        {
            using (_database)
            {
                var serviceCallLineItem = _database.SingleById<ServiceCallLineItem>(message.ServiceCallLineItemId);
                var serviceCall = _database.SingleById<ServiceCall>(serviceCallLineItem.ServiceCallId);

                _bus.Publish<ServiceCallLineItemCompleted>(x =>
                {
                    x.ServiceCallNumber = serviceCall.ServiceCallNumber;
                    x.LineNumber = serviceCallLineItem.LineNumber;
                    x.ServiceCallLineItemStatus = serviceCallLineItem.ServiceCallLineItemStatus.DisplayName;
                });
            }
        }
    }
}