namespace Warranty.Core.Features.Report.WSRLoadingReport
{
    using System;
    using System.Collections.Generic;

    public class WSRLoadingReportModel
    {
        public IEnumerable<LoadingSummary> LoadingSummaries { get; set; }
        public IEnumerable<EmployeeTiedToRepresentative> EmployeeTiedToRepresentatives { get; set; }

        public string SelectedEmployeeNumber { get; set; }
        public bool AnyResults { get; set; }
        public int TotalNumberOfWarrantableHomes { get; set; }

        public class LoadingSummary
        {
            public string EmployeeNumber { get; set; }
            public string EmployeeName { get; set; }
            public Guid CommunityId { get; set; }
            public string CommunityName { get; set; }
            public int NumberOfWarrantableHomes { get; set; }
        }

        public class EmployeeTiedToRepresentative
        {
            public Guid WarrantyRepresentativeEmployeeId { get; set; }
            public string EmployeeNumber { get; set; }
            public string EmployeeName { get; set; }
        }
    }
}