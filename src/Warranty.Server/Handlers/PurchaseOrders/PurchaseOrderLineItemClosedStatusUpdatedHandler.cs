namespace Warranty.Server.Handlers.PurchaseOrders
{
    using Accounting.Events.PurchaseOrders;
    using Core.Entities;
    using Core.Enumerations;
    using Extensions;
    using NPoco;
    using NServiceBus;

    public class PurchaseOrderLineItemClosedStatusUpdatedHandler : IHandleMessages<PurchaseOrderLineItemClosed>
    {
        private readonly IDatabase _database;

        public PurchaseOrderLineItemClosedStatusUpdatedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(PurchaseOrderLineItemClosed message)
        {
            using (_database)
            {
                var purchaseOrderLineItem = _database.SingleOrDefaultByJdeId<PurchaseOrderLineItem>(message.PurchaseOrderJDEId);

                if (purchaseOrderLineItem == null)
                    return;
                
                purchaseOrderLineItem.PurchaseOrderLineItemStatus = PurchaseOrderLineItemStatus.Closed;
                _database.Update(purchaseOrderLineItem);
            }
        }
    }
}