namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

    public class CityCodeProblemCodeCostCodeMap : AuditableEntityMap<CityCodeProblemCodeCostCode>
    {
        public CityCodeProblemCodeCostCodeMap()
        {
            TableName("CityCodeProblemCodeCostCodes")
                .PrimaryKey("CityCodeProblemCodeCostCodeId", false)
                .Columns(x =>
                {
                    x.Column(y => y.CityCode);
                    x.Column(y => y.ProblemJdeCode);
                    x.Column(y => y.CostCode);
                });
        }
    }
}
