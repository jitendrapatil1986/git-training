namespace Warranty.Server.Handlers.PurchaseOrders
{
    using Accounting.Events.PurchaseOrders;
    using Core.Entities;
    using Extensions;
    using NPoco;
    using NServiceBus;

    public class PurchaseOrderLineItemDescriptionUpdatedHandler : IHandleMessages<PurchaseOrderLineItemDescriptionUpdated>
    {
        private readonly IDatabase _database;

        public PurchaseOrderLineItemDescriptionUpdatedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(PurchaseOrderLineItemDescriptionUpdated message)
        {
            using (_database)
            {
                var purchaseOrderLineItem = _database.SingleOrDefaultByJdeId<PurchaseOrderLineItem>(message.JDEId);

                if (purchaseOrderLineItem == null)
                    return;

                purchaseOrderLineItem.Description = message.PurchaseOrderLineItemDescription;
                _database.Update(purchaseOrderLineItem);
            }
        }
    }
}