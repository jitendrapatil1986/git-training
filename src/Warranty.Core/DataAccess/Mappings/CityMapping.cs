namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;
    using NHibernate.Mapping.ByCode;

    public class CityMapping : AuditableEntityMapping<City>
    {
        public CityMapping()
        {
            Table("Cities");

            Id(x => x.CityId, map => map.Generator(Generators.GuidComb));
            Property(x => x.CityCode);
            Property(x => x.CityName);
        }
    }

    public class CityMap : AuditableEntityMap<City>
    {
        public CityMap()
        {
            TableName("Cities");

            PrimaryKey(x => x.CityId, false);
            Columns(x =>
                        {
                            x.Column(col => col.CityCode);
                            x.Column(col => col.CityName);
                        });
        }
    }
}