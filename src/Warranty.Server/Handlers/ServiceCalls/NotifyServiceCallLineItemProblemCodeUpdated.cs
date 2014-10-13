namespace Warranty.Server.Handlers.ServiceCalls
{
    using Core.Entities;
    using Events;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyServiceCallLineItemProblemUpdatedHandler : IHandleMessages<NotifyServiceCallLineItemProblemUpdated>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;

        public NotifyServiceCallLineItemProblemUpdatedHandler(IBus bus, IDatabase database)
        {
            _bus = bus;
            _database = database;
        }

        public void Handle(NotifyServiceCallLineItemProblemUpdated message)
        {
            using (_database)
            {
                var serviceCallLineItem = _database.SingleById<ServiceCallLineItem>(message.ServiceCallLineItemId);
                var serviceCall = _database.SingleById<ServiceCall>(serviceCallLineItem.ServiceCallId);

                _bus.Publish<ServiceCallLineItemProblemUpdated>(x =>
                {
                    x.ServiceCallNumber = serviceCall.ServiceCallNumber;
                    x.LineNumber = serviceCallLineItem.LineNumber;
                    x.ProbemDescription = serviceCallLineItem.ProblemDescription;
                    x.ProblemCode = serviceCallLineItem.ProblemCode;
                });
            }
        }
    }
}