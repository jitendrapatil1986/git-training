namespace Warranty.Server.Handlers.PurchaseOrders
{
    using Accounting.Events.PurchaseOrders;
    using Core.Entities;
    using Extensions;
    using NPoco;
    using NServiceBus;

    public class PurchaseOrderLineItemDeletedHandler : IHandleMessages<PurchaseOrderLineDeleted>
    {
        private readonly IDatabase _database;

        public PurchaseOrderLineItemDeletedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(PurchaseOrderLineDeleted message)
        {
            using (_database)
            {
                var purchaseOrderLineItem = _database.SingleOrDefaultByJdeId<PurchaseOrderLineItem>(message.JDEId);

                if (purchaseOrderLineItem == null)
                    return;

                _database.Delete(purchaseOrderLineItem);
            }
        }
    }
}