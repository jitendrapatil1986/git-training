using NPoco;
using StructureMap.Configuration.DSL;
using Warranty.Core.DataAccess;

namespace Warranty.Server.IntegrationTests
{
    public class WarrantyRegistry : Registry
    {
        public WarrantyRegistry()
        {
            For<IDatabase>().Use(() => DbFactory.DatabaseFactory.GetDatabase());

            Scan(scanner =>
            {
                scanner.WithDefaultConventions();

                scanner.TheCallingAssembly();
            });
        }
    }
}