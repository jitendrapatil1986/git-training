namespace Warranty.Server.Handlers.PurchaseOrders
{
    using Accounting.Events.PurchaseOrders;
    using Core.Entities;
    using Core.Enumerations;
    using Extensions;
    using NPoco;
    using NServiceBus;

    public class PurchaseOrderLineItemCancelledStatusUpdatedHandler : IHandleMessages<PurchaseOrderLineItemCanceled>
    {
        private readonly IDatabase _database;

        public PurchaseOrderLineItemCancelledStatusUpdatedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(PurchaseOrderLineItemCanceled message)
        {
            using (_database)
            {
                var purchaseOrderLineItem = _database.SingleOrDefaultByJdeId<PurchaseOrderLineItem>(message.PurchaseOrderJDEId);

                if (purchaseOrderLineItem == null)
                    return;
                
                purchaseOrderLineItem.PurchaseOrderLineItemStatus = PurchaseOrderLineItemStatus.Cancelled;
                _database.Update(purchaseOrderLineItem);
            }
        }
    }
}