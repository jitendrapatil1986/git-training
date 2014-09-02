using System.Configuration;
using NPoco.FluentMappings;
using Warranty.Core.Security;

namespace Warranty.Core.DataAccess
{
    using NPoco;
    using StructureMap;
    using System.Linq;

    public static class DbFactory
    {
        public static DatabaseFactory DatabaseFactory { get; set; }

        public static void Setup(IContainer container, IUserSession userSession)
        {
            var maps = container.GetAllInstances<IMap>().ToArray();
            var fluentConfig = FluentMappingConfiguration.Configure(maps);

            var dbType = new SqlServerDatabaseType();
            var connString = ConfigurationManager.ConnectionStrings["WarrantyDB"].ConnectionString;

            var dataBase = new SqlServerDatabase(connString, dbType, userSession);

            DatabaseFactory = DatabaseFactory.Config(x =>
            {
                x.UsingDatabase(() => dataBase);
                x.WithFluentConfig(fluentConfig);
                x.WithMapper(new EnumerationMapper());
            });
        }
    }
}
