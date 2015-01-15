namespace Warranty.Core.Features.Report.WSRSummary
{
    using System.Collections.Generic;
    using System.Linq;

    public class WSRSummaryModel
    {
        public IEnumerable<WSRSummaryLine> WSRSummaryLines { get; set; }
        public int TotalNumberOfWarrantableHomes { get { return WSRSummaryLines.Sum(x => x.NumberOfWarrantableHomes); } }
        public int TotalNumberOfNonWarrantableHomes { get { return WSRSummaryLines.Sum(x => x.NumberOfNonWarrantableHomes); } }
        public int TotalNumberOfOpenServiceCalls { get { return WSRSummaryLines.Sum(x => x.NumberOfOpenServiceCalls); } }
        public bool AnyResults { get { return WSRSummaryLines.Any(); } }

        public class WSRSummaryLine
        {
            public string EmployeeNumber { get; set; }
            public string EmployeeName { get; set; }
            public int NumberOfOpenServiceCalls { get; set; }
            public int NumberOfWarrantableHomes { get; set; }
            public int NumberOfNonWarrantableHomes { get; set; }
        }
    }
}