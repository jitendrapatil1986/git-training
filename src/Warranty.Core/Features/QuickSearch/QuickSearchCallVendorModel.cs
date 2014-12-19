namespace Warranty.Core.Features.QuickSearch
{
    using System;

    public class QuickSearchCallVendorModel
    {
        public Guid Id { get; set; }
        public string VendorNumber { get; set; }
        public string VendorName { get; set; }
        public string HeldStatus { get; set; }
        public bool VendorOnHold { get { return String.Equals(HeldStatus, "Y", StringComparison.CurrentCultureIgnoreCase); } }
        public string InvoiceProcessingCode { get; set; }
    }
}