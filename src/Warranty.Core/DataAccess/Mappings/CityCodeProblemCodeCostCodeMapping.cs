namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;
    using NHibernate.Mapping.ByCode;

    public class CityCodeProblemCodeCostCodeMapping : AuditableEntityMapping<CityCodeProblemCodeCostCode>
    {
        public CityCodeProblemCodeCostCodeMapping()
        {
            Table("CityCodeProblemCodeCostCodes");
            Id(x=>x.CityCodeProblemCodeCostCodeId, map => map.Generator(Generators.GuidComb));
            Property(x => x.CityCode);
            Property(x => x.ProblemJdeCode);
            Property(x => x.CostCode);
        }
    }
}
