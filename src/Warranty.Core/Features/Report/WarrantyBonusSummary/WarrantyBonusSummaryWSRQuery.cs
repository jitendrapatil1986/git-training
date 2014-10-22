namespace Warranty.Core.Features.Report.WarrantyBonusSummary
{
    public class WarrantyBonusSummaryWSRQuery : IQuery<WarrantyBonusSummaryModel>
    {
        public WarrantyBonusSummaryModel Model { get; set; }

        public WarrantyBonusSummaryWSRQuery()
        {
            Model = new WarrantyBonusSummaryModel();
        }
    }
}