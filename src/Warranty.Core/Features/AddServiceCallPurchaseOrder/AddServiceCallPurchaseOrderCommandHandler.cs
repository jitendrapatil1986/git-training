namespace Warranty.Core.Features.AddServiceCallPurchaseOrder
{
    using Entities;
    using Enumerations;
    using NPoco;

    public class AddServiceCallPurchaseOrderCommandHandler : ICommandHandler<AddServiceCallPurchaseOrderCommand>
    {
        private readonly IDatabase _database;

        public AddServiceCallPurchaseOrderCommandHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(AddServiceCallPurchaseOrderCommand message)
        {
            using (_database)
            {
                //TODO: Update with correct fields.
                var purchaseOrder = new PurchaseOrder
                    {
                        CostCode = message.Model.CostCode,
                        //DeliveryDate = message.Model.DeliveryDate,
                        DeliveryInstructions = DeliveryInstruction.ASAP, //message.Model.DeliveryInstructions,
                        JobNumber = message.Model.JobNumber,
                        ObjectAccount = message.Model.ObjectAccount,
                        PurchaseOrderNote = message.Model.PurchaseOrderNote,
                        ServiceCallLineItemId = message.Model.ServiceCallLineItemId,
                        VendorName = message.Model.VendorName,
                        VendorNumber = message.Model.VendorNumber,
                    };

                _database.Insert(purchaseOrder);
                
                foreach (var line in message.Model.ServiceCallLineItemPurchaseOrderLines)
                {
                    var purchaseOrderLineItem = new PurchaseOrderLineItem
                        {
                            Description = line.Description,
                            LineNumber = line.LineNumber,
                            Quantity = line.Quantity,
                            UnitCost = line.UnitCost,
                            PurchaseOrderLineItemStatus = PurchaseOrderLineItemStatus.Open,
                            PurchaseOrderId = purchaseOrder.PurchaseOrderId,
                        };

                    if(purchaseOrderLineItem.Quantity != 0 && !string.IsNullOrEmpty(purchaseOrderLineItem.Description) && purchaseOrderLineItem.UnitCost != 0)
                        _database.Insert(purchaseOrderLineItem);
                }
            }
        }
    }
}