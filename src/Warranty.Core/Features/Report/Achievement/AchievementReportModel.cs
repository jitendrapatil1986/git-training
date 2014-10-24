namespace Warranty.Core.Features.Report.Achievement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;

    public class AchievementReportModel
    {
        public AchievementReportModel()
        {
            AchievementSummaries = new List<AchievementSummary>();
        }
        public IEnumerable<AchievementSummary> AchievementSummaries { get; set; }
        public IEnumerable<EmployeeTiedToRepresentative> EmployeeTiedToRepresentatives { get; set; }

        public string SelectedEmployeeNumber { get; set; }
        public bool AnyResults { get { return AchievementSummaries.Any(); } }
        public bool HasSearchCriteria { get { return FilteredDate.HasValue; } }
        public DateTime? FilteredDate { get; set; }

        public DateTime StartDate
        {
            get
            {
                if (FilteredDate.HasValue)
                    return FilteredDate.Value.AddYears(-1).ToFirstDay();
                return DateTime.Today.AddYears(-1).ToFirstDay();
            }
        }

        public DateTime EndDate
        {
            get
            {
                if (FilteredDate.HasValue)
                    return FilteredDate.Value.ToLastDay();

                return DateTime.Today.ToLastDay();
            }
        }

        public class AchievementSummary
        {
            public decimal AmountSpentPerHome { get; set; }
            public decimal DWR { get; set; }
            public decimal RTFT { get; set; }
            public decimal EWS { get; set; }
            public decimal PercentComplete7Days { get; set; }
            public decimal AverageDaysClosing { get; set; }
            public int Month { get; set; }
            public int Year { get; set; }
        }

        public class EmployeeTiedToRepresentative
        {
            public Guid WarrantyRepresentativeEmployeeId { get; set; }
            public string EmployeeNumber { get; set; }
            public string EmployeeName { get; set; }
        }
    }
}