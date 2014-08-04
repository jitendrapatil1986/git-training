using NHibernate;
using StructureMap.Configuration.DSL;
using Warranty.LotusExtract.Security;

namespace Warranty.LotusExtract
{
    public class WarrantyRegistry : Registry
    {
        public WarrantyRegistry()
        {
            Scan(scanner =>
            {
                scanner.WithDefaultConventions();

                scanner.TheCallingAssembly();
            });

            For<ISessionFactory>().Singleton().Use(new ConfigurationFactory().CreateConfigurationWithAuditing(new ImporterUserSession()).BuildSessionFactory());
            For<ISession>().Use(ctx => ctx.GetInstance<ISessionFactory>().OpenSession());
        }
    }
}