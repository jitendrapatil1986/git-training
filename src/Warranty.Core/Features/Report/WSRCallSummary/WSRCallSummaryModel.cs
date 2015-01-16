namespace Warranty.Core.Features.Report.WSRCallSummary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class WSRCallSummaryModel
    {
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public IEnumerable<ServiceCall> ServiceCalls { get; set; }
        public bool AnyResults { get { return ServiceCalls.Any(); } }
        public int TotalNumberOfOpenServiceCalls { get { return ServiceCalls.Count(); } }
        
        public class ServiceCall
        {
            public Guid ServiceCallId { get; set; }
            public string ServiceCallNumber { get; set; }
            public string HomeownerName { get; set; }
            public string CommunityName { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string StateCode { get; set; }
            public string PostalCode { get; set; }
            public string HomePhone { get; set; }
            public string OtherPhone { get; set; }
            public List<ServiceCallLine> ServiceCallLines { get; set; }
            public IEnumerable<ServiceCallNote> ServiceCallNotes { get; set; }
        }

        public class ServiceCallLine
        {
            public Guid ServiceCallId { get; set; }
            public int LineNumber { get; set; }
            public string ProblemCode { get; set; }
            public string ProblemDescription { get; set; }
        }

        public class ServiceCallNote
        {
            public Guid ServiceCallId { get; set; }
            public string Note { get; set; }
        }
    }
}