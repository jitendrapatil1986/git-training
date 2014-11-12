using NPoco;
using Warranty.Core.DataAccess;
using Warranty.Core.Security;
using Warranty.Server.Security;

namespace Warranty.Server
{
    using Core.Entities;
    using NPoco.FluentMappings;
    using StructureMap.Configuration.DSL;

    public class WarrantyRegistry : Registry
    {
        public WarrantyRegistry()
        {
            Scan(scanner =>
            {
                scanner.WithDefaultConventions();
                
                scanner.TheCallingAssembly();
                scanner.AssemblyContainingType<IAuditableEntity>();
                scanner.AddAllTypesOf<IMap>();
                For<IDatabase>().Use(() => DbFactory.DatabaseFactory.GetDatabase());
                For<IUserSession>().Use<WarrantyServerUserSession>();
            });
        }
    }
}
