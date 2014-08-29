namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

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