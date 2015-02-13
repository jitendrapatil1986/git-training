namespace Warranty.Server.IntegrationTests.SetUp
{
    using System;
    using Core.Entities;
    using NPoco;

    public class CityEntityBuilder : EntityBuilder<City>
    {
        public CityEntityBuilder(IDatabase database)
            : base(database)
        {
        }

        public override City GetSaved(Action<City> action)
        {
            var entity = new City
                             {
                                 CityCode = "IND",
                                 CreatedBy = "test",
                                 CreatedDate = DateTime.UtcNow,
                             };

            return Saved(entity, action);
        }
    }
}