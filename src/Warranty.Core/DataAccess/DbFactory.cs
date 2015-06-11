using System.Configuration;
using NPoco.FluentMappings;
using Common.Security.Session;

namespace Warranty.Core.DataAccess
{
    using NPoco;
    using StructureMap;
    using System.Linq;

    public static class DbFactory
    {
        public static DatabaseFactory DatabaseFactory { get; set; }

        public static void Setup(IContainer container)
        {
            var maps = container.GetAllInstances<IMap>().ToArray();
            var fluentConfig = FluentMappingConfiguration.Configure(maps);

            DatabaseFactory = DatabaseFactory.Config(x =>
            {
                x.UsingDatabase(() =>
                {
                    var dbType = new SqlServerDatabaseType();
                    var connString = ConfigurationManager.ConnectionStrings["WarrantyDB"].ConnectionString;

                    return new SqlServerDatabase(connString, dbType, container.GetInstance<IUserSession>());
                });
                x.WithFluentConfig(fluentConfig);
                x.WithMapper(new EnumerationMapper());
            });
        }
    }
}
