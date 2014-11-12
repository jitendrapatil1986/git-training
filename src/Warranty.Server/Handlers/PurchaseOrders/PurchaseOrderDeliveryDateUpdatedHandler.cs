namespace Warranty.Server.Handlers.PurchaseOrders
{
    using Accounting.Events.PurchaseOrders;
    using Core.Entities;
    using Extensions;
    using NPoco;
    using NServiceBus;

    public class PurchaseOrderDeliveryDateUpdatedHandler : IHandleMessages<PurchaseOrderDeliveryDateUpdated>
    {
        private readonly IDatabase _database;

        public PurchaseOrderDeliveryDateUpdatedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(PurchaseOrderDeliveryDateUpdated message)
        {
            using (_database)
            {
                var purchaseOrder = _database.SingleOrDefaultByJdeId<PurchaseOrder>(message.JDEId);

                if (purchaseOrder == null)
                    return;

                purchaseOrder.DeliveryDate = message.DeliveryConfirmDate;
                _database.Update(purchaseOrder);
            }
        }
    }
}