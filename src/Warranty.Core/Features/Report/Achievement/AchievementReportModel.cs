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

        public class AchievementDto
        {
            public decimal Amount { get; set; }
            public int MonthNumber { get; set; }
            public int YearNumber { get; set; }
        }

        public class SurveyDataResult
        {
            public string DefinitelyWillRecommend { get; set; }
            public string ExcellentWarrantyService { get; set; }
            public string RightFirstTime { get; set; }
            public DateTime SurveyDate { get; set; }
        }

        public class EmployeeTiedToRepresentative
        {
            public Guid WarrantyRepresentativeEmployeeId { get; set; }
            public string EmployeeNumber { get; set; }
            public string EmployeeName { get; set; }
        }
    }
}