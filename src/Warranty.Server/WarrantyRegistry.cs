using NHibernate;
using NServiceBus.UnitOfWork;
using StructureMap.Configuration.DSL;
using Warranty.Core.DataAccess;

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

            For<ISessionFactory>().Singleton().Use(new ConfigurationFactory().CreateConfiguration().BuildSessionFactory());
            For<ISession>().Use(ctx => ctx.GetInstance<ISessionFactory>().OpenSession());
            For<IManageUnitsOfWork>().Use<NHibernateUnitOfWork>();
        }
    }
}