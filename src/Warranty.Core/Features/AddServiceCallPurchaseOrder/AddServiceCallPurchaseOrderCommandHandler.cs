namespace Warranty.Core.Features.AddServiceCallPurchaseOrder
{
    using Configurations;
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
                //TODO: Job keeps failing b/c IsOutOfWarranty is not a valid column???
                var job = _database.SingleOrDefault<Job>("WHERE JobNumber = @0", message.Model.JobNumber);
                
                var purchaseOrder = new PurchaseOrder
                    {
                        CostCode = WarrantyCostCode.FromValue(message.Model.CostCode).DisplayName,
                        DeliveryDate = message.Model.DeliveryDate,
                        DeliveryInstructions = DeliveryInstruction.FromValue(message.Model.DeliveryInstructions),
                        JobNumber = message.Model.JobNumber,
                        PurchaseOrderNote = message.Model.PurchaseOrderNote,
                        ServiceCallLineItemId = message.Model.ServiceCallLineItemId,
                        VendorName = message.Model.VendorName,
                        VendorNumber = message.Model.VendorNumber,
                        ObjectAccount = job.IsOutOfWarranty
                                            ? message.Model.ObjectAccount.Equals(WarrantyConstants.MaterialObjectAccount)
                                                  ? WarrantyConstants.OurOfWarrantyMaterialCode
                                                  : WarrantyConstants.OutOfWarrantyLaborCode
                                            : message.Model.ObjectAccount.Equals(WarrantyConstants.MaterialObjectAccount)
                                                  ? WarrantyConstants.InWarrantyMaterialCode
                                                  : WarrantyConstants.InWarrantyLaborCode,
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