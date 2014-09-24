namespace Warranty.Core.Features.CompleteServiceCallLineItem
{
    using System;
    using Entities;
    using Enumerations;
    using NPoco;

    public class CompleteServiceCallLineItemCommandHandler : ICommandHandler<CompleteServiceCallLineItemCommand, ServiceCallLineItemStatus>
    {
        private IDatabase _database;

        public CompleteServiceCallLineItemCommandHandler(IDatabase database)
        {
            _database = database;
        }

        public ServiceCallLineItemStatus Handle(CompleteServiceCallLineItemCommand message)
        {
            using (_database)
            {
                var completeServiceCallLineItem = _database.SingleById<ServiceCallLineItem>(message.ServiceCallLineItemId);
                completeServiceCallLineItem.ServiceCallLineItemStatus = ServiceCallLineItemStatus.Closed;
                _database.Update(completeServiceCallLineItem);

                return completeServiceCallLineItem.ServiceCallLineItemStatus;
            }

        }
    }
}