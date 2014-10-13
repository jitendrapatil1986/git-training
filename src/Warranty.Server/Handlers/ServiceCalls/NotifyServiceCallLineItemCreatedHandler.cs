namespace Warranty.Server.Handlers.ServiceCalls
{
    using Core.Entities;
    using Events;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyServiceCallLineItemCreatedHandler : IHandleMessages<NotifyServiceCallLineItemCreated>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;

        public NotifyServiceCallLineItemCreatedHandler(IBus bus, IDatabase database)
        {
            _bus = bus;
            _database = database;
        }

        public void Handle(NotifyServiceCallLineItemCreated message)
        {
            using (_database)
            {
                var serviceCallLineItem = _database.SingleById<ServiceCallLineItem>(message.ServiceCallLineItemId);
                var serviceCall = _database.SingleById<ServiceCall>(serviceCallLineItem.ServiceCallId);

                _bus.Publish<ServiceCallLineItemCreated>(x =>
                    {
                        x.ServiceCallNumber = serviceCall.ServiceCallNumber;
                        x.CauseDescription = serviceCallLineItem.CauseDescription;
                        x.ClassificationNote = serviceCallLineItem.ClassificationNote;
                        x.LineItemRoot = serviceCallLineItem.LineItemRoot;
                        x.LineNumber = serviceCallLineItem.LineNumber;
                        x.ProblemCode = serviceCallLineItem.ProblemCode;
                        x.ProblemDescription = serviceCallLineItem.ProblemDescription;
                        x.ServiceCallNumber = serviceCall.ServiceCallNumber;
                        x.ServiceCallLineItemStatus = serviceCallLineItem.ServiceCallLineItemStatus.DisplayName;
                    });
            }
        }
    }
}