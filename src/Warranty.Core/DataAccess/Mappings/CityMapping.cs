using NHibernate.Mapping.ByCode.Conformist;
using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess.Mappings
{
    public class CityMapping : ClassMapping<City>
    {
        public CityMapping()
        {
            Table("Cities");

            Id(x => x.CityId);
            Property(x => x.CityCode);
            Property(x => x.CityName);
            Property(x => x.CreatedDate);
            Property(x => x.CreatedBy);
            Property(x => x.UpdatedDate);
            Property(x => x.UpdatedBy);
        }
    }
}