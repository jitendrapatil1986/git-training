﻿namespace Warranty.Core.DataAccess
{
    using System.Data;
    using NHibernate.Caches.SysCache;
    using NHibernate.Cfg;
    using NHibernate.Dialect;
    using NHibernate.Driver;
    using Security;

    public class ConfigurationFactory : IConfigurationFactory
    {
        public Configuration CreateConfiguration()
        {
            var nhConfiguration = ConfigureNHibernate();
            var mappings = new HbmMappingGenerator().GenerateMappings();
            nhConfiguration.AddDeserializedMapping(mappings, "Warranty");
            return nhConfiguration;
        }

        public Configuration CreateConfigurationWithAuditing(IUserSession session)
        {
            return CreateConfiguration().AttachAuditEventListeners(session);
        }

        private static Configuration ConfigureNHibernate()
        {
            var configure = new Configuration();
            configure.SessionFactoryName("Warranty");

            configure.DataBaseIntegration(db =>
            {
                db.Dialect<MsSql2008Dialect>();
                db.Driver<SqlClientDriver>();
                db.IsolationLevel = IsolationLevel.Snapshot;
                if (System.Environment.GetEnvironmentVariable("db_connstring") != null)
                {
                    db.ConnectionString = System.Environment.GetEnvironmentVariable("db_connstring");
                }
                else
                {
                    db.ConnectionStringName = "WarrantyDB";
                }
                db.Timeout = 10;
                db.BatchSize = 100;
                db.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
                db.LogFormattedSql = true;
            });
            configure.SessionFactory()
                .GeneratingCollections.Through<Net4CollectionTypeFactory>()
                .GenerateStatistics();
            configure.Cache(p =>
            {
                p.Provider<SysCacheProvider>();
                p.UseQueryCache = true;
            });
            return configure;
        }
    }
}
