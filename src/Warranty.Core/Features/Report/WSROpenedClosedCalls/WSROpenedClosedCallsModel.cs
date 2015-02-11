namespace Warranty.Core.Features.Report.WSROpenedClosedCalls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class WSROpenedClosedCallsModel
    {
        public WSROpenedClosedCallsModel()
        {
            WSRSummaryLines = new List<WSRSummaryLine>();
        }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public IEnumerable<WSRSummaryLine> WSRSummaryLines { get; set; }
        public int TotalCallsBeforeTimeframe { get { return WSRSummaryLines.Sum(x => x.NumberCallsBeforeTimeframe); } }
        public int TotalCallsOpenedDuringTimeframe { get { return WSRSummaryLines.Sum(x => x.NumberCallsOpenedDuringTimeframe); } }
        public int TotalCallsClosedDuringTimeframe { get { return WSRSummaryLines.Sum(x => x.NumberCallsClosedDuringTimeframe); } }
        public int TotalCallsAfterTimeframe { get { return WSRSummaryLines.Sum(x => x.NumberCallsAfterTimeframe); } }
        public bool AnyResults { get { return WSRSummaryLines.Any(); } }

        public class WSRSummaryLine
        {
            public string EmployeeNumber { get; set; }
            public string EmployeeName { get; set; }
            public int NumberCallsBeforeTimeframe { get; set; }
            public int NumberCallsOpenedDuringTimeframe { get; set; }
            public int NumberCallsClosedDuringTimeframe { get; set; }
            public int NumberCallsAfterTimeframe { get; set; }
        }
    }
}