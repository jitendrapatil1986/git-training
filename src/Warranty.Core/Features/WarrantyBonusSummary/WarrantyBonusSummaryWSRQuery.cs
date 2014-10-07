namespace Warranty.Core.Features.WarrantyBonusSummary
{
    using System;

    public class WarrantyBonusSummaryWSRQuery : IQuery<WarrantyBonusSummaryModel>
    {
        public string EmployeeNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}