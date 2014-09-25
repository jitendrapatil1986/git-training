namespace Warranty.Core.Features.UpdateServiceCallLineItem
{
    using Entities;
    using Enumerations;
    using NPoco;

    public class UpdateServiceCallLineItemCommanHandler : ICommandHandler<UpdateServiceCallLineItemCommand, ServiceCallLineItemStatus>
    {
        private readonly IDatabase _database;

        public UpdateServiceCallLineItemCommanHandler(IDatabase database)
        {
            _database = database;
        }

        public ServiceCallLineItemStatus Handle(UpdateServiceCallLineItemCommand message)
        {
            using (_database)
            {
                var reopenServiceCallLineItem = _database.SingleById<ServiceCallLineItem>(message.ServiceCallLineItemId);
                reopenServiceCallLineItem.ServiceCallLineItemStatus = ServiceCallLineItemStatus.Open;
                _database.Update(reopenServiceCallLineItem);

                return reopenServiceCallLineItem.ServiceCallLineItemStatus;
            }
        }
    }
}