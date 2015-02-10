namespace Warranty.Core.Features.NoActionServiceCallLineItem
{
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using NPoco;

    public class NoActionServiceCallLineItemCommandHandler : ICommandHandler<NoActionServiceCallLineItemCommand, ServiceCallLineItemStatus>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _logger;

        public NoActionServiceCallLineItemCommandHandler(IDatabase database, IActivityLogger logger)
        {
            _database = database;
            _logger = logger;
        }

        public ServiceCallLineItemStatus Handle(NoActionServiceCallLineItemCommand message)
        {
            using (_database)
            {
                var serviceCallLineItem = _database.SingleById<ServiceCallLineItem>(message.ServiceCallLineItemId);
                var currentLineItemStatus = serviceCallLineItem.ServiceCallLineItemStatus;
                serviceCallLineItem.ServiceCallLineItemStatus = ServiceCallLineItemStatus.NoAction;

                _database.Update(serviceCallLineItem);

                const string activityName = "Service call line item was set to no action.";
                var details = "Previous line item status: " + currentLineItemStatus.DisplayName;

                _logger.Write(activityName, details, message.ServiceCallLineItemId, ActivityType.NoAction, ReferenceType.ServiceCallLineItem);

                return serviceCallLineItem.ServiceCallLineItemStatus;
            }
        }
    }
}