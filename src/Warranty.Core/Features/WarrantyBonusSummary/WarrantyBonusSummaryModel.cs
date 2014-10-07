namespace Warranty.Core.Features.WarrantyBonusSummary
{
    using System.Collections.Generic;

    public class WarrantyBonusSummaryModel
    {
        public IEnumerable<BonusSummary> BonusSummaries { get; set; }

        public class BonusSummary
        {
            //public string EmployeeName { get; set; }
            //public string EmployeeNumber { get; set; }
            //public int Month { get; set; }
            //public int Year { get; set; }
            //public string Division { get; set; }
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
    }
}