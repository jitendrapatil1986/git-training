namespace Warranty.Core.Features.WarrantyBonusSummary
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

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
        public int TotalNumberOfWarrantableHomes { get; set; }
        public decimal TotalMaterialDollarsSpent { get; set; }
        public decimal TotalLaborDollarsSpent { get; set; }
        public decimal TotalOtherMaterialDollarsSpent { get; set; }
        public decimal TotalOtherLaborDollarsSpent { get; set; }
        public decimal TotalDollarsSpent { get; set; }
        public decimal TotalWarrantyAllowance { get; set; }
        public decimal TotalWarrantyDifference { get; set; }
        public decimal TotalDefinitelyWouldRecommendSurveyBonusAmount { get; set; }
        public decimal TotalExcellentWarrantySurveyBonusAmount { get; set; }
        public decimal TotalAllItemsCompleteBonusAmount { get; set; }
        public decimal TotalMiscellaneousBonusAmount { get; set; }
        public decimal TotalRepresentativeBonusAmount { get; set; }

        public class BonusSummary
        {
            public string EmployeeName { get; set; }
            public string EmployeeNumber { get; set; }
            //public int Month { get; set; }
            //public int Year { get; set; }
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
            public Guid CustomerId { get; set; }
            public string CustomerName { get; set; }
            public Guid JobId { get; set; }
            public string JobName { get; set; }
            public bool DefinitelyWouldRecommend { get; set; }
            public decimal BonusAmount { get; set; }
        }

        public class ExcellentWarrantySurvey
        {
            public Guid CustomerId { get; set; }
            public string CustomerName { get; set; }
            public Guid JobId { get; set; }
            public string JobName { get; set; }
            public bool ExcellentWarranty { get; set; }
            public decimal BonusAmount { get; set; }
        }

        public class ItemsComplete
        {
            public Guid CommunityId { get; set; }
            public string CommunityName { get; set; }
            public decimal CompletePercentage { get; set; }
        }

        public class MiscellaneousBonus
        {
            public string Description { get; set; }
            public decimal BonusAmount { get; set; }
        }
    }
}