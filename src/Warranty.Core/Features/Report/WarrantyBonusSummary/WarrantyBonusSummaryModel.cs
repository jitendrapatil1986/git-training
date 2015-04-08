namespace Warranty.Core.Features.Report.WarrantyBonusSummary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configurations;
    using Extensions;

    public class WarrantyBonusSummaryModel
    {
        public WarrantyBonusSummaryModel()
        {
            BonusSummaries = new List<BonusSummary>();
            EmployeeTiedToRepresentatives = new List<EmployeeTiedToRepresentative>();
            DefinitelyWouldRecommendSurveys = new List<DefinitelyWouldRecommendSurvey>();
            ExcellentWarrantySurveys = new List<ExcellentWarrantySurvey>();
            AllItemsCompletes = new List<ItemsComplete>();
        }

        public IEnumerable<BonusSummary> BonusSummaries { get; set; }
        public IEnumerable<EmployeeTiedToRepresentative> EmployeeTiedToRepresentatives { get; set; }
        public IEnumerable<DefinitelyWouldRecommendSurvey> DefinitelyWouldRecommendSurveys { get; set; }
        public IEnumerable<ExcellentWarrantySurvey> ExcellentWarrantySurveys { get; set; }
        public IEnumerable<ItemsComplete> AllItemsCompletes { get; set; }
        public string SelectedEmployeeNumber { get; set; }
        public DateTime? FilteredDate { get; set; }

        public DateTime StartDate
        {
            get
            {
                if (FilteredDate.HasValue)
                    return FilteredDate.Value.ToFirstDay();
                return DateTime.Today.ToFirstDay();
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

        public string EmployeeName { get; set; }
        public string EmployeeNumber { get; set; }
        public string DivisionName { get; set; }
        public int TotalNumberOfWarrantableHomes { get { return BonusSummaries.Sum(x => x.NumberOfWarrantableHomes); }}
        public decimal TotalMaterialDollarsSpent { get { return BonusSummaries.Sum(x => x.MaterialDollarsSpent); }}
        public decimal TotalLaborDollarsSpent { get { return BonusSummaries.Sum(x => x.LaborDollarsSpent); }}
        public decimal TotalOtherMaterialDollarsSpent { get { return BonusSummaries.Sum(x => x.OtherMaterialDollarsSpent); }}
        public decimal TotalOtherLaborDollarsSpent { get { return BonusSummaries.Sum(x => x.OtherLaborDollarsSpent); }}
        public decimal TotalDollarsSpent { get { return BonusSummaries.Sum(x => x.TotalDollarsSpent); }}
        public decimal TotalWarrantyAllowance { get { return BonusSummaries.Sum(x => x.TotalWarrantyAllowance); }}
        public decimal TotalWarrantyDifference { get { return BonusSummaries.Sum(x => x.TotalWarrantyDifference); }}
        public decimal TotalCostControlBonusAmount { get { return TotalWarrantyDifference * SurveyConstants.CostControlBonusPercent; }}
        
        public decimal TotalDefinitelyWouldRecommendSurveyBonusAmount 
        {
            get
            {
                return DefinitelyWouldRecommendSurveys.Count(x => x.IsBonusable) * SurveyConstants.DefinitelyWillBonusAmount;
            }
        }
        
        public decimal TotalExcellentWarrantySurveyBonusAmount 
        { 
            get
            {
            return
                ExcellentWarrantySurveys.Count(x => x.IsBonusable) * SurveyConstants.ExcellentWarrantyBonusAmount;
            } 
        }

        public bool AnyResults
        {
            get
            {
                return BonusSummaries.Any() || DefinitelyWouldRecommendSurveys.Any() || ExcellentWarrantySurveys.Any() || AllItemsCompletes.Any();
            }
        }

        public decimal TotalAllItemsCompletePercent { get { return AllItemsCompletes.Any() ? Math.Round(AllItemsCompletes.Sum(x => x.CompletePercentage * x.Count)/AllItemsCompletes.Sum(x => x.Count), 2) : 0; }}
        public decimal TotalAllItemsCompleteBonusAmount { get { return IsBonusable ? SurveyConstants.AllItemsCompleteBonusAmount : 0; }}
        public bool IsBonusable { get { return TotalAllItemsCompletePercent >= SurveyConstants.AllItemsCompletePercentThreshold; }}

        public class BonusSummary
        {
            public string EmployeeName { get; set; }
            public string EmployeeNumber { get; set; }
            public string DivisionName { get; set; }
            public string CommunityName { get; set; }
            public int NumberOfWarrantableHomes { get; set; }
            public decimal MaterialDollarsSpent { get; set; }
            public decimal LaborDollarsSpent { get; set; }
            public decimal OtherMaterialDollarsSpent { get; set; }
            public decimal OtherLaborDollarsSpent { get; set; }
            public decimal TotalDollarsSpent { get; set; }
            public decimal TotalWarrantyAllowance { get; set; }
            public decimal TotalWarrantyDifference { get; set; }
        }

        public class EmployeeTiedToRepresentative
        {
            public Guid WarrantyRepresentativeEmployeeId { get; set; }
            public string EmployeeNumber { get; set; }
            public string EmployeeName { get; set; }
        }

        public class DefinitelyWouldRecommendSurvey
        {
            public string HomeownerName { get; set; }
            public string JobNumber { get; set; }
            public string DefinitelyWillRecommend { get; set; }
            public decimal BonusAmount { get { return  IsBonusable ? SurveyConstants.DefinitelyWillBonusAmount : 0; }}
            public bool IsBonusable 
            {
                get
                {
                    return string.Equals(DefinitelyWillRecommend, SurveyConstants.DefinitelyWillThreshold, StringComparison.CurrentCultureIgnoreCase);
                }
            }
        }

        public class ExcellentWarrantySurvey
        {
            public string HomeownerName { get; set; }
            public string JobNumber { get; set; }
            public string ExcellentWarrantyService { get; set; }
            public decimal BonusAmount { get { return IsBonusable ? SurveyConstants.ExcellentWarrantyBonusAmount : 0; }}
            public bool IsBonusable 
            {
                get
                { 
                    return Convert.ToInt16(ExcellentWarrantyService) >= SurveyConstants.ExcellentWarrantyThreshold;
                }
            }
        }

        public class ItemsComplete
        {
            public string CommunityName { get; set; }
            public decimal CompletePercentage { get; set; }
            public int Count { get; set; }
        }
    }
}