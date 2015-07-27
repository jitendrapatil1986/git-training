namespace Warranty.Core.Features.Report.Saltline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;

    public class SaltlineReportModel
    {
        public SaltlineReportModel()
        {
            DivisionSaltlineSummary = new List<SaltlineSummary>();
            EmployeeSaltlineSummary = new List<SaltlineSummary>();
            ProjectSaltlineSummary = new List<SaltlineSummary>();
        }
        public IList<SaltlineSummary> EmployeeSaltlineSummary { get; set; }
        public IList<SaltlineSummary> DivisionSaltlineSummary { get; set; }
        public IList<SaltlineSummary> ProjectSaltlineSummary { get; set; }

        public string SelectedEmployeeNumber { get; set; }
        public bool AnyResults { get { return EmployeeSaltlineSummary.Any() || DivisionSaltlineSummary.Any() || ProjectSaltlineSummary.Any(); } }
        public bool HasSearchCriteria { get { return StartDate.HasValue & EndDate.HasValue; } }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public class SaltlineSummary
        {
            public decimal AmountSpentPerHome { get; set; }
            public decimal? DefinetelyWouldRecommend { get; set; }
            public decimal? RightTheFirstTime { get; set; }
            public decimal? OutstandingWarrantyService { get; set; }
            public decimal PercentComplete7Days { get; set; }
            public decimal AverageDaysClosing { get; set; }
            public int Month { get; set; }
            public int Year { get; set; }
            public string EmployeeName { get; set; }
            public string EmployeeNumber { get; set; }
            public string DivisionName { get; set; }
            public string DivisionId { get; set; }
            public string ProjectNumber { get; set; }
            public string ProjectName { get; set; }
            public decimal NumerOfCalls { get; set; }
            public decimal NumberOfSurveys { get; set; }
            public decimal NumberOfHomes { get; set; }
        }

        public class EmployeeModel
        {
            public string EmployeeNumber { get; set; }
            public string EmployeeName { get; set; }
        }

        public class DivisionModel
        {
            public string DivisionCode { get; set; }
            public string DivisionName { get; set; }
        }

        public class ProjectModel
        {
            public string ProjectNumber { get; set; }
            public string ProjectName { get; set; }
        }
    }
}