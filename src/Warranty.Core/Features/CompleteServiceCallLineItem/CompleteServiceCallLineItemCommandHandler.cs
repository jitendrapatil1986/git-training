namespace Warranty.Core.Features.CompleteServiceCallLineItem
{
    using System;
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class CompleteServiceCallLineItemCommandHandler : ICommandHandler<CompleteServiceCallLineItemCommand, ServiceCallLineItemStatus>
    {
        private readonly IDatabase _database;
        private readonly IBus _bus;
        private readonly IActivityLogger _logger;

        public CompleteServiceCallLineItemCommandHandler(IDatabase database, IBus bus, IActivityLogger logger)
        {
            _database = database;
            _bus = bus;
            _logger = logger;
        }

        public ServiceCallLineItemStatus Handle(CompleteServiceCallLineItemCommand message)
        {
            using (_database)
            {
                var completeServiceCallLineItem = _database.SingleById<ServiceCallLineItem>(message.ServiceCallLineItemId);
                var currentLineItemStatus = completeServiceCallLineItem.ServiceCallLineItemStatus;
                completeServiceCallLineItem.ServiceCallLineItemStatus = ServiceCallLineItemStatus.Complete;
                completeServiceCallLineItem.LastCompletedDate = DateTime.Now;
                _database.Update(completeServiceCallLineItem);

                _bus.Send<NotifyServiceCallLineItemCompleted>(x =>
                    {
                        x.ServiceCallLineItemId = completeServiceCallLineItem.ServiceCallLineItemId;
                    });

                const string activityName = "Service call line item was completed.";
                var details = "Previous line item status: " + currentLineItemStatus.DisplayName;

                _logger.Write(activityName, details, message.ServiceCallLineItemId, ActivityType.Complete, ReferenceType.ServiceCallLineItem);

                return completeServiceCallLineItem.ServiceCallLineItemStatus;
            }

        }
    }
}