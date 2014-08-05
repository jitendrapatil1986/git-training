using NHibernate.Mapping.ByCode.Conformist;
using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess.Mappings
{
    public class CityMapping : AuditableEntityMapping<City>
    {
        public CityMapping()
        {
            Table("Cities");

            Id(x => x.CityId);
            Property(x => x.CityCode);
            Property(x => x.CityName);
        }
    }
}