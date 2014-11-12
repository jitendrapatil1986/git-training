namespace Warranty.Core.Entities
{
    using System;

    public class PurchaseOrderLineItem : IAuditableEntity, IJdeEntity
    {
        public Guid PurchaseOrderLineItemId { get; set; }
        public Guid PurchaseOrderId { get; set; }
        public int LineNumber { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public string Description { get; set; }
        public int PurchaseOrderLineItemStatusId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string JdeIdentifier { get; set; }
    }
}