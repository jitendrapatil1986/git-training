namespace Warranty.Server.Handlers.PurchaseOrders
{
    using Accounting.Events.PurchaseOrders;
    using Core.Entities;
    using Core.Enumerations;
    using Extensions;
    using NPoco;
    using NServiceBus;

    public class PurchaseOrderDeliveryInstructionsUpdatedHandler : IHandleMessages<PurchaseOrderDeliveryInstructionsUpdated>
    {
        private readonly IDatabase _database;

        public PurchaseOrderDeliveryInstructionsUpdatedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(PurchaseOrderDeliveryInstructionsUpdated message)
        {
            using (_database)
            {
                var purchaseOrder = _database.SingleOrDefaultByJdeId<PurchaseOrder>(message.JDEId);

                if (purchaseOrder == null)
                    return;

                purchaseOrder.DeliveryInstructions = DeliveryInstruction.FromJdeCode(message.PurchaseOrderDeliveryInstructions);
                _database.Update(purchaseOrder);
            }
        }
    }
}