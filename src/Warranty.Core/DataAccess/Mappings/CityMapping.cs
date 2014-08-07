namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;
    using NHibernate.Mapping.ByCode;

    public class CityMapping : AuditableEntityMapping<City>
    {
        public CityMapping()
        {
            Table("Cities");

            Id(x => x.CityId, map => map.Generator(new GuidCombGeneratorDef()));
            Property(x => x.CityCode);
            Property(x => x.CityName);
        }
    }
}