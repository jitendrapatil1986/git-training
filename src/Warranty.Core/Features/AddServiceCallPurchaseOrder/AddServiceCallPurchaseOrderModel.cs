namespace Warranty.Core.Features.AddServiceCallPurchaseOrder
{
    using System;
    using System.Collections.Generic;
    using Enumerations;
    using ServiceCallSummary.ServiceCallLineItem;

    public class AddServiceCallPurchaseOrderModel
    {
        public string VendorNumber { get; set; }
        public string VendorName { get; set; }
        public DeliveryInstruction DeliveryInstructions { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string CostCode { get; set; }
        public string ObjectAccount { get; set; }
        public Guid ServiceCallLineItemId { get; set; }
        public string JobNumber { get; set; }
        public string PurchaseOrderNote { get; set; }
        public List<ServiceCallLineItemModel.ServiceCallLineItemPurchaseOrderLine> ServiceCallLineItemPurchaseOrderLines { get; set; }
    }

    public class ServiceCallLineItemPurchaseOrderLine
    {
        public int LineNumber { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public string Description { get; set; }
        public PurchaseOrderLineItemStatus PurchaseOrderLineItemStatus { get; set; }
    }
}