namespace Warranty.Server.Handlers.PurchaseOrders
{
    using Accounting.Events.PurchaseOrders;
    using Core.Entities;
    using Extensions;
    using NPoco;
    using NServiceBus;

    public class PurchaseOrderDeletedHandler : IHandleMessages<PurchaseOrderDeleted>
    {
        private readonly IDatabase _database;

        public PurchaseOrderDeletedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(PurchaseOrderDeleted message)
        {
            using (_database)
            {
                var purchaseOrder = _database.SingleOrDefaultByJdeId<PurchaseOrder>(message.JDEId);

                if (purchaseOrder == null)
                    return;

                _database.DeleteMany<PurchaseOrderLineItem>().Where(x => x.PurchaseOrderId == purchaseOrder.PurchaseOrderId).Execute();
                _database.Delete(purchaseOrder);
            }
        }
    }
}