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
            MonthlyAchievementSummary = new List<AchievementSummary>();
        }
        public IEnumerable<AchievementSummary> MonthlyAchievementSummary { get; set; }
        public AchievementSummary PeriodAchievementSummary { get; set; }
        public IEnumerable<EmployeeTiedToRepresentative> EmployeeTiedToRepresentatives { get; set; }

        public string SelectedEmployeeNumber { get; set; }
        public bool AnyResults { get { return MonthlyAchievementSummary.Any(); } }
        public bool HasSearchCriteria { get { return StartDate.HasValue & EndDate.HasValue; } }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

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