namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

    public class JobVendorCostCodeMap : AuditableEntityMap<JobVendorCostCode>
    {
        public JobVendorCostCodeMap()
        {
            TableName("JobVendorCostCodes")
                .PrimaryKey("JobVendorCostCodeId", false)
                .Columns(x =>
                    {
                        x.Column(y => y.VendorId);
                        x.Column(y => y.JobId);
                        x.Column(y => y.CostCode);
                        x.Column(y => y.CostCodeDescription);
                    });
        }
    }
}