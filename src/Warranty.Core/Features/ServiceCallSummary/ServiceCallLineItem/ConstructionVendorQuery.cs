namespace Warranty.Core.Features.ServiceCallSummary.ServiceCallLineItem
{
    using System.Collections.Generic;

    public class ConstructionVendorQuery : IQuery<IEnumerable<ConstructionVendorModel>>
    {
        public string JobNumber;
        public string CostCode;
    }
}
