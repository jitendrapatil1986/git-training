namespace Warranty.Server
{
    using Core.DataAccess;
    using NHibernate;
    using NServiceBus.UnitOfWork;
    using StructureMap.Configuration.DSL;

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