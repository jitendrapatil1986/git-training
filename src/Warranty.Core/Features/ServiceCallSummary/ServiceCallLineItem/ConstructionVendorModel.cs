namespace Warranty.Core.Features.ServiceCallSummary.ServiceCallLineItem
{
    using System.Collections.Generic;

    public class ConstructionVendorModel
    {
        public string VendorName { get; set; }
        public string VendorNumber { get; set; }
        public string VendorEmail { get; set; }
        public IEnumerable<string> VendorPhoneNumbers { get; set; }
    }
}

