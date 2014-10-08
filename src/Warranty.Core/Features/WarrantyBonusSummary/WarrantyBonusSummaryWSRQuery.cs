namespace Warranty.Core.Features.WarrantyBonusSummary
{
    using System;

    public class WarrantyBonusSummaryWSRQuery : IQuery<WarrantyBonusSummaryModel>
    {
        //public string EmployeeNumber { get; set; }
        //public DateTime FilterDate { get; set; }
        public WarrantyBonusSummaryModel queryModel { get; set; }
    }
}