namespace Warranty.Core.Features.CompleteServiceCallLineItem
{
    using System;
    using Entities;
    using Enumerations;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class CompleteServiceCallLineItemCommandHandler : ICommandHandler<CompleteServiceCallLineItemCommand, ServiceCallLineItemStatus>
    {
        private IDatabase _database;
        private readonly IBus _bus;

        public CompleteServiceCallLineItemCommandHandler(IDatabase database, IBus bus)
        {
            _database = database;
            _bus = bus;
        }

        public ServiceCallLineItemStatus Handle(CompleteServiceCallLineItemCommand message)
        {
            using (_database)
            {
                var completeServiceCallLineItem = _database.SingleById<ServiceCallLineItem>(message.ServiceCallLineItemId);
                completeServiceCallLineItem.ServiceCallLineItemStatus = ServiceCallLineItemStatus.Complete;
                _database.Update(completeServiceCallLineItem);

                _bus.Send<NotifyServiceCallLineItemCompleted>(x =>
                    {
                        x.ServiceCallLineItemId = completeServiceCallLineItem.ServiceCallLineItemId;
                    });

                return completeServiceCallLineItem.ServiceCallLineItemStatus;
            }

        }
    }
}