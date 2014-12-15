using NPoco;
using Warranty.Core.DataAccess;
using Warranty.Core.Security;
using Warranty.Server.Security;

namespace Warranty.Server
{
    using Accounting.Client;
    using Core.Entities;
    using NPoco.FluentMappings;
    using StructureMap.Configuration.DSL;
    using StructureMap.Pipeline;

    public class WarrantyRegistry : Registry
    {
        public WarrantyRegistry()
        {
            Scan(scanner =>
            {
                scanner.WithDefaultConventions();
                
                scanner.TheCallingAssembly();
                scanner.AssemblyContainingType<IAuditableEntity>();
                scanner.AddAllTypesOf((typeof(IAccountingClient)));
                scanner.AddAllTypesOf<IMap>();

                For<IDatabase>()
                    .LifecycleIs(new ThreadLocalStorageLifecycle())
                    .Use(() => DbFactory.DatabaseFactory.GetDatabase());

                For<IUserSession>().Use<WarrantyServerUserSession>();
            });
        }
    }
}
