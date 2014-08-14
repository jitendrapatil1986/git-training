using NPoco.FluentMappings;
using Warranty.Core.DataAccess.Mappings;

namespace Warranty.Core.DataAccess
{
    using NPoco;

    public static class DbFactory
    {
        public static DatabaseFactory DatabaseFactory { get; set; }

        public static void Setup()
        {
            var fluentConfig = FluentMappingConfiguration.Configure(new PaymentMapping());

            DatabaseFactory = DatabaseFactory.Config(x =>
            {
                x.UsingDatabase(() => new Database("WarrantyDB"));
                x.WithFluentConfig(fluentConfig);
            });
        }
    }
}
