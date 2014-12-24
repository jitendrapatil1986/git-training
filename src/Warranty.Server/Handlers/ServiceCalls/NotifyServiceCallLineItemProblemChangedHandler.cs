namespace Warranty.Server.Handlers.ServiceCalls
{
    using Core.Entities;
    using Events;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyServiceCallLineItemProblemChangedHandler : IHandleMessages<NotifyServiceCallLineItemProblemChanged>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;

        public NotifyServiceCallLineItemProblemChangedHandler(IBus bus, IDatabase database)
        {
            _bus = bus;
            _database = database;
        }

        public void Handle(NotifyServiceCallLineItemProblemChanged message)
        {
            using (_database)
            {
                var serviceCallLineItem = _database.SingleById<ServiceCallLineItem>(message.ServiceCallLineItemId);

                _bus.Publish<ServiceCallLineItemProblemChanged>(x =>
                {
                    x.ServiceCallId = serviceCallLineItem.ServiceCallId;
                    x.ServiceCallLineItemId = serviceCallLineItem.ServiceCallLineItemId;
                    x.ProbemDescription = serviceCallLineItem.ProblemDescription;
                    x.ProblemCode = serviceCallLineItem.ProblemCode;
                    x.ProblemJdeCode = serviceCallLineItem.ProblemJdeCode;
                    x.RootCause = serviceCallLineItem.RootCause;
                    x.RootProblem = serviceCallLineItem.RootProblem;
                });
            }
        }
    }
}