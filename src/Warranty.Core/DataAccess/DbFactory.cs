using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using NPoco.DatabaseTypes;
using NPoco.FluentMappings;
using Warranty.Core.DataAccess.Mappings;
using Warranty.Core.Entities;
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

        public class SqlServerDatabaseType : SqlServer2012DatabaseType
        {
            public SqlServerDatabaseType()
            {
                AddTypeMap(typeof(DateTime), DbType.DateTime2);
                AddTypeMap(typeof(string), DbType.AnsiString);
            }
        }

        public class SqlServerDatabase : Database
        {
            private readonly IUserSession _userSession;

            public SqlServerDatabase(string connectionString, DatabaseType databaseType, IUserSession userSession)
                : base(connectionString, databaseType)
            {
                _userSession = userSession;
            }

            protected override bool OnUpdating(UpdateContext updateContext)
            {
                var poco = updateContext.Poco as IAuditableEntity;

                if (poco != null)
                {
                    poco.UpdatedDate = DateTime.Now;
                    poco.UpdatedBy = _userSession.GetCurrentUser().UserName;
                }

                return base.OnUpdating(updateContext);
            }

            protected override bool OnInserting(InsertContext insertContext)
            {
                var poco = insertContext.Poco as IAuditableEntity;
                
                if (poco != null)
                {
                    poco.CreatedDate = DateTime.Now;
                    poco.CreatedBy = _userSession.GetCurrentUser().UserName;
                }

                return base.OnInserting(insertContext);
            }
        }
    }
}
