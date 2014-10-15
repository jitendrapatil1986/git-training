namespace Warranty.Core.Features.Report.WarrantyBonusSummary
{
    public class WarrantyBonusSummaryConfig
    {
        public const decimal CostControlBonusPercent = 0.10m;
        public const decimal AllItemsCompletePercentThreshold = 85;
        public const decimal AllItemsCompleteBonusAmount = 100;
        public const string DefinitelyWillThreshold = "DEFINITELY WILL";
        public const decimal DefinitelyWillBonusAmount = 50;
        public const int ExcellentWarrantyThreshold = 9;
        public const decimal ExcellentWarrantyBonusAmount = 50;
        public const string AllItemsCompleteThreshold = "YES";
    }
}