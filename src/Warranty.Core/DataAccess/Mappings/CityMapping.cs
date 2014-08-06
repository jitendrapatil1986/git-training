using NHibernate.Mapping.ByCode;
using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess.Mappings
{
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