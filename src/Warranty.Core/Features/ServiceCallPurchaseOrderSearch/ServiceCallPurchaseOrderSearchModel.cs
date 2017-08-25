namespace Warranty.Core.Features.ServiceCallPurchaseOrderSearch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Entities;
    using Enumerations;
    using System.ComponentModel.DataAnnotations;

    public class ServiceCallPurchaseOrderSearchModel
    {
        public string PurchaseOrderNumber { get; set; }
        public int? VendorNumber { get; set; }
        public string VendorName { get; set; }
        public Vendor Vendor { get; set; }
        public int? JobNumber { get; set; }
        public string HomeOwnerName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ThruDate { get; set; }
        public int? PurchaseOrderLineItemStatus { get; set; }
        public IList<PurchaseOrderDetail> Results { get; set; }

        public int ResultCount
        {
            get { return (Results != null && Results.Any()) ? Results.Count() : 0; }
        }

        //this is here in case it ever needs to be a user input
        public int MaxResults
        {
            get { return 100; }
        }

        public bool HasSearchCriteria()
        {
            return FromDate.HasValue || ThruDate.HasValue || VendorNumber.HasValue || JobNumber.HasValue || !string.IsNullOrEmpty(PurchaseOrderNumber) || PurchaseOrderLineItemStatus.HasValue;
        }

        public ServiceCallPurchaseOrderSearchModel()
        {
            Results = new List<PurchaseOrderDetail>();
        }

        public class PurchaseOrderDetail
        {
            public Guid ServiceCallLineItemId { get; set; }
            public Guid PurchaseOrderId { get; set; }
            public string PurchaseOrderNumber { get; set; }
            public Guid JobId { get; set; }
            public string JobNumber { get; set; }
            public string AddressLine { get; set; }
            public string VendorNumber { get; set; }
            public string VendorName { get; set; }
            public bool IsOpen { get; set; }
            public int PurchaseOrderLineItemStatusId { get; set; }

        }
    }
}
