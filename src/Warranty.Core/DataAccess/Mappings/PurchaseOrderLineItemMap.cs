namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

    public class PurchaseOrderLineItemMap : AuditableEntityMap<PurchaseOrderLineItem>
    {
        public PurchaseOrderLineItemMap()
        {
            TableName("PurchaseOrderLineItems")
                .PrimaryKey("PurchaseOrderLineItemId", false)
                .Columns(x =>
                    {
                        x.Column(y => y.Description);
                        x.Column(y => y.JdeIdentifier);
                        x.Column(y => y.LineNumber);
                        x.Column(y => y.PurchaseOrderId);
                        x.Column(y => y.PurchaseOrderLineItemStatus).WithName("PurchaseOrderLineItemStatusId");
                        x.Column(y => y.Quantity);
                        x.Column(y => y.UnitCost);
                    });
        }
    }
}