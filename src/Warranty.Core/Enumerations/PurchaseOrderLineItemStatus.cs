namespace Warranty.Core.Enumerations
{
    public class PurchaseOrderLineItemStatus : Enumeration<PurchaseOrderLineItemStatus>
    {
        public PurchaseOrderLineItemStatus(int value, string displayName)
            : base(value, displayName)
        {
        }

        public static readonly PurchaseOrderLineItemStatus Open = new PurchaseOrderLineItemStatus(1, "Open");
        public static readonly PurchaseOrderLineItemStatus Closed = new PurchaseOrderLineItemStatus(2, "Closed");
        public static readonly PurchaseOrderLineItemStatus Cancelled = new PurchaseOrderLineItemStatus(3, "Cancelled");
    }
}