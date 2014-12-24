namespace Warranty.Core.Entities
{
    using System;
    using Enumerations;

    public class PurchaseOrder : IAuditableEntity, IJdeEntity
    {
        public Guid PurchaseOrderId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string VendorNumber { get; set; }
        public string VendorName { get; set; }
        public DeliveryInstruction DeliveryInstructions { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public WarrantyCostCode CostCode { get; set; }
        public string ObjectAccount { get; set; }
        public Guid ServiceCallLineItemId { get; set; }
        public string JobNumber { get; set; }
        public string PurchaseOrderNote { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string JdeIdentifier { get; set; }
    }
}