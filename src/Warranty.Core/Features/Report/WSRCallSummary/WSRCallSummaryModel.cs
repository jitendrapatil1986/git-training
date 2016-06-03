namespace Warranty.Core.Features.Report.WSRCallSummary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class WSRCallSummaryModel
    {
        public WSRCallSummaryModel()
        {
            EmployeeTiedToRepresentatives = new List<EmployeeTiedToRepresentative>();
            ServiceCalls = new List<ServiceCall>();
        }

        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public IEnumerable<EmployeeTiedToRepresentative> EmployeeTiedToRepresentatives { get; set; }
        public string SelectedEmployeeNumber { get; set; }
        public IEnumerable<ServiceCall> ServiceCalls { get; set; }
        public bool AnyResults { get { return ServiceCalls.Any(); } }
        public int TotalNumberOfOpenServiceCalls { get { return ServiceCalls.Count(); } }

        public class EmployeeTiedToRepresentative
        {
            public Guid WarrantyRepresentativeEmployeeId { get; set; }
            public string EmployeeNumber { get; set; }
            public string EmployeeName { get; set; }
        }
    }
}