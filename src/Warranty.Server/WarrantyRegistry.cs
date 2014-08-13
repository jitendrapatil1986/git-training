using NPoco;
using Warranty.Core.DataAccess;

namespace Warranty.Server
{
    using StructureMap.Configuration.DSL;

    public class WarrantyRegistry : Registry
    {
        public WarrantyRegistry()
        {
            Scan(scanner =>
            {
                scanner.WithDefaultConventions();
                
                scanner.TheCallingAssembly();

                For<IDatabase>().Use(() => DbFactory.DatabaseFactory.GetDatabase());
            });
        }
    }
}