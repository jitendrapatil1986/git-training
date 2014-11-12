namespace Warranty.Server.Handlers.PurchaseOrders
{
    using Accounting.Events.PurchaseOrders;
    using Core.Entities;
    using Core.Enumerations;
    using Extensions;
    using NPoco;
    using NServiceBus;

    public class PurchaseOrderLineItemOpenStatusUpdatedHandler : IHandleMessages<PurchaseOrderLineItemOpen>
    {
        private readonly IDatabase _database;

        public PurchaseOrderLineItemOpenStatusUpdatedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(PurchaseOrderLineItemOpen message)
        {
            using (_database)
            {
                var purchaseOrderLineItem = _database.SingleOrDefaultByJdeId<PurchaseOrderLineItem>(message.PurchaseOrderJDEId);

                if (purchaseOrderLineItem == null)
                    return;
                
                purchaseOrderLineItem.PurchaseOrderLineItemStatus = PurchaseOrderLineItemStatus.Open;
                _database.Update(purchaseOrderLineItem);
            }
        }
    }
}