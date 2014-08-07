namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

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