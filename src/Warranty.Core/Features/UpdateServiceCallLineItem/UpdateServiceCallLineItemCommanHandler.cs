namespace Warranty.Core.Features.UpdateServiceCallLineItem
{
    using Entities;
    using Enumerations;
    using NPoco;

    public class UpdateServiceCallLineItemCommanHandler : ICommandHandler<UpdateServiceCallLineItemCommand, UpdateServiceCallLineItemModel>
    {
        private readonly IDatabase _database;

        public UpdateServiceCallLineItemCommanHandler(IDatabase database)
        {
            _database = database;
        }

        public UpdateServiceCallLineItemModel Handle(UpdateServiceCallLineItemCommand message)
        {
            using (_database)
            {
                var reopenServiceCallLineItem = _database.SingleById<ServiceCallLineItem>(message.ServiceCallLineItemId);
                reopenServiceCallLineItem.ServiceCallLineItemStatus = ServiceCallLineItemStatus.Open;
                _database.Update(reopenServiceCallLineItem);

                var serviceCall = _database.SingleById<ServiceCall>(reopenServiceCallLineItem.ServiceCallId);
                if (serviceCall.ServiceCallStatus == ServiceCallStatus.Complete || serviceCall.ServiceCallStatus == ServiceCallStatus.HomeownerSigned)
                {
                    serviceCall.ServiceCallStatus = ServiceCallStatus.Open;
                    serviceCall.CompletionDate = null;
                    _database.Update(serviceCall);
                }

                var model = new UpdateServiceCallLineItemModel
                    {
                        ServiceCallLineItemId = reopenServiceCallLineItem.ServiceCallId,
                        ServiceCallLineItemStatus = reopenServiceCallLineItem.ServiceCallLineItemStatus,
                        ServiceCallStatus = serviceCall.ServiceCallStatus,
                    };

                return model;
            }
        }
    }
}