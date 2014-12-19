namespace Warranty.Core.Features.AddServiceCallPurchaseOrder
{
    using System;
    using System.Collections.Generic;
    using Enumerations;

    public class AddServiceCallPurchaseOrderModel
    {
        public Guid JobId { get; set; }
        public Guid ServiceCallLineItemId { get; set; }
        public int ServiceCallNumber { get; set; }
        public string JobNumber { get; set; }
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string StateCode { get; set; }
        public string PostalCode { get; set; }
        public string CityCode { get; set; }
        
        public decimal? PurchaseOrderMaxAmount { get; set; }
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