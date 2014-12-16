namespace Warranty.Core.Features.AddServiceCallPurchaseOrder
{
    using System;
    using System.Collections.Generic;

    public class AddServiceCallPurchaseOrderCommand : ICommand
    {
        public Guid ServiceCallLineItemId { get; set; }
        public string VendorNumber { get; set; }
        public string VendorName { get; set; }
        public int DeliveryInstructions { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int CostCode { get; set; }
        public string PurchaseOrderNote { get; set; }
        public bool IsMaterialObjectAccount { get; set; }
        public List<ServiceCallLineItemPurchaseOrderLine> ServiceCallLineItemPurchaseOrderLines { get; set; }
    }
}