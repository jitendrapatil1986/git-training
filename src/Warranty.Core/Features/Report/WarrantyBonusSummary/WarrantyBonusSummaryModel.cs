namespace Warranty.Core.Features.Report.WarrantyBonusSummary
{
    using System;
    using System.Collections.Generic;

    public class WarrantyBonusSummaryModel
    {
        public IEnumerable<BonusSummary> BonusSummaries { get; set; }
        public IEnumerable<EmployeeTiedToRepresentative> EmployeeTiedToRepresentatives { get; set; }
        public IEnumerable<DefinitelyWouldRecommendSurvey> DefinitelyWouldRecommendSurveys { get; set; }
        public IEnumerable<ExcellentWarrantySurvey> ExcellentWarrantySurveys { get; set; }
        public IEnumerable<ItemsComplete> AllItemsCompletes { get; set; }
        public IEnumerable<MiscellaneousBonus> MiscellaneousBonuses { get; set; }

        public string SelectedEmployeeNumber { get; set; }
        public DateTime? FilteredDate { get; set; }
        public bool AnyResults { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeNumber { get; set; }
        public string DivisionName { get; set; }
        public int TotalNumberOfWarrantableHomes { get; set; }
        public decimal TotalMaterialDollarsSpent { get; set; }
        public decimal TotalLaborDollarsSpent { get; set; }
        public decimal TotalOtherMaterialDollarsSpent { get; set; }
        public decimal TotalOtherLaborDollarsSpent { get; set; }
        public decimal TotalDollarsSpent { get; set; }
        public decimal TotalWarrantyAllowance { get; set; }
        public decimal TotalWarrantyDifference { get; set; }
        public decimal TotalCostControlBonusAmount { get; set; }
        public decimal TotalDefinitelyWouldRecommendSurveyBonusAmount { get; set; }
        public decimal TotalExcellentWarrantySurveyBonusAmount { get; set; }
        public decimal TotalAllItemsCompletePercent { get; set; }
        public decimal TotalAllItemsCompleteBonusAmount { get; set; }
        public decimal TotalMiscellaneousBonusAmount { get; set; }
        public decimal TotalRepresentativeBonusAmount { get; set; }

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
            public int ElevenMonthWarrantySurveyId { get; set; }
            public string HomeownerName { get; set; }
            public string JobNumber { get; set; }
            public DateTime SurveyDate { get; set; }
            public string WarrantyServiceRepresentativeEmployeeId { get; set; }
            public string WarrantyServiceRepresentativeName { get; set; }
            public string DefinitelyWillRecommend { get; set; }
            public decimal BonusAmount { get; set; }
        }

        public class ExcellentWarrantySurvey
        {
            public int ElevenMonthWarrantySurveyId { get; set; }
            public string HomeownerName { get; set; }
            public string JobNumber { get; set; }
            public DateTime SurveyDate { get; set; }
            public string WarrantyServiceRepresentativeEmployeeId { get; set; }
            public string WarrantyServiceRepresentativeName { get; set; }
            public string ExcellentWarrantyService { get; set; }
            public decimal BonusAmount { get; set; }
        }

        public class ItemsComplete
        {
            public Guid CommunityId { get; set; }
            public string CommunityName { get; set; }
            public decimal CompletePercentage { get; set; }
            public int ElevenMonthWarrantySurveyId { get; set; }
            public string JobNumber { get; set; }
            public DateTime SurveyDate { get; set; }
            public string ItemsCompleted { get; set; }
        }

        public class MiscellaneousBonus
        {
            public string Description { get; set; }
            public decimal BonusAmount { get; set; }
        }
    }
}