namespace Warranty.Core.Features.ServiceCallPurchaseOrderDetail
{
    using System;
    using System.Collections.Generic;
    using Enumerations;
    using System.Linq;

    public class ServiceCallPurchaseOrderDetailModel
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
        public ServiceCallLineItemPurchaseOrder ServiceCallLineItemPurchaseOrders { get; set; }

        public class ServiceCallLineItemPurchaseOrder
        {
            public Guid PurchaseOrderId { get; set; }
            public string PurchaseOrderNumber { get; set; }
            public string VendorNumber { get; set; }
            public string VendorName { get; set; }
            public WarrantyCostCode CostCode { get; set; }
            public DateTime? CreatedDate { get; set; }
            public string ObjectAccount { get; set; }
            public bool IsMaterialObjectAccount { get; set; }
            public List<ServiceCallLineItemPurchaseOrderLine> ServiceCallLineItemPurchaseOrderLines { get; set; }
            public decimal TotalCost { get { return ServiceCallLineItemPurchaseOrderLines.Sum(x => x.Quantity * x.UnitCost); } }
            public PurchaseOrderLineItemStatus PurchaseOrderStatus
            {
                get
                {
                    return ServiceCallLineItemPurchaseOrderLines.All(x => x.PurchaseOrderLineItemStatus.Equals(PurchaseOrderLineItemStatus.Closed)) ? PurchaseOrderLineItemStatus.Closed :
                        PurchaseOrderLineItemStatus.Open;
                }
            }

            public string PurchaseOrderStatusDisplayName { get { return PurchaseOrderStatus.DisplayName; } }
            public string DeliveryInstructions { get; set; }
            public string PurchaseOrderNote { get; set; }
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

   
}