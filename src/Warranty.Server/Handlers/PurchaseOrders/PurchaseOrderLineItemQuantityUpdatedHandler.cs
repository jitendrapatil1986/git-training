namespace Warranty.Server.Handlers.PurchaseOrders
{
    using Accounting.Events.PurchaseOrders;
    using Core.Entities;
    using Extensions;
    using NPoco;
    using NServiceBus;

    public class PurchaseOrderLineItemQuantityUpdatedHandler : IHandleMessages<PurchaseOrderLineQuantityUpdated>
    {
        private readonly IDatabase _database;

        public PurchaseOrderLineItemQuantityUpdatedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(PurchaseOrderLineQuantityUpdated message)
        {
            using (_database)
            {
                var purchaseOrderLineItem = _database.SingleOrDefaultByJdeId<PurchaseOrderLineItem>(message.JDEId);

                if (purchaseOrderLineItem == null)
                    return;
                
                purchaseOrderLineItem.Quantity = message.PurchaseOrderLineQuantity;
                _database.Update(purchaseOrderLineItem);
            }
        }
    }
}