namespace Warranty.Server.Handlers.PurchaseOrders
{
    using Accounting.Events.PurchaseOrders;
    using Core.Entities;
    using NPoco;
    using NServiceBus;

    public class RequestPurchaseOrderResponseHandler : IHandleMessages<RequestPurchaseOrderResponse>
    {
        private readonly IDatabase _database;

        public RequestPurchaseOrderResponseHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(RequestPurchaseOrderResponse message)
        {
            using (_database)
            {
                var purchaseOrder = _database.SingleById<PurchaseOrder>(message.PurchaseOrderIdentifier);
                
                purchaseOrder.PurchaseOrderNumber = message.PurchaseOrderNumber;
                purchaseOrder.JdeIdentifier = message.PurchaseOrderJdeIdentifier;
                
                _database.Update(purchaseOrder);
            }
        }
    }
}