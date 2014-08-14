using System.Configuration;
using System.Data.Common;
using NPoco.FluentMappings;
using Warranty.Core.DataAccess.Mappings;
using Warranty.Core.Security;

namespace Warranty.Core.DataAccess
{
    using NPoco;

    public static class DbFactory
    {
        public static DatabaseFactory DatabaseFactory { get; set; }

        public static void Setup(IUserSession userSession)
        {
            var fluentConfig = FluentMappingConfiguration.Configure(new EntityMapper());

            var dbType = new SqlServerDatabaseType();
            var connString = ConfigurationManager.ConnectionStrings["WarrantyDB"].ConnectionString;

            var dataBase = new SqlServerDatabase(connString, dbType, userSession);

            DatabaseFactory = DatabaseFactory.Config(x =>
            {
                x.UsingDatabase(() => dataBase);
                x.WithFluentConfig(fluentConfig);
            });
        }
    }
}
