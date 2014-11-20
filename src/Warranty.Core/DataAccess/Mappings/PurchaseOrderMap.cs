namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

    public class PurchaseOrderMap : AuditableEntityMap<PurchaseOrder>
    {
        public PurchaseOrderMap()
        {
            TableName("PurchaseOrders")
                .PrimaryKey("PurchaseOrderId", false)
                .Columns(x =>
                {
                    x.Column(y => y.CostCode);
                    x.Column(y => y.DeliveryDate);
                    x.Column(y => y.DeliveryInstructions);
                    x.Column(y => y.JdeIdentifier);
                    x.Column(y => y.JobNumber);
                    x.Column(y => y.ObjectAccount);
                    x.Column(y => y.PurchaseOrderNote);
                    x.Column(y => y.PurchaseOrderNumber);
                    x.Column(y => y.ServiceCallLineItemId);
                    x.Column(y => y.VendorName);
                    x.Column(y => y.VendorNumber);
                });
        }
    }
}