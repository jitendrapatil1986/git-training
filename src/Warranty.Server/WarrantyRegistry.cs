using NHibernate;
using NServiceBus.UnitOfWork;
using StructureMap.Configuration.DSL;

namespace Warranty.Server
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

            //For<ISessionFactory>().Singleton().Use(new ConfigurationFactory(new WarrantyRepositoryConfiguration(), new WarrantyBusUserSession()).GetConfiguration().BuildSessionFactory());
            For<ISession>().Use(ctx => ctx.GetInstance<ISessionFactory>().OpenSession());
            For<IManageUnitsOfWork>().Use<NHibernateUnitOfWork>();
        }
    }
}