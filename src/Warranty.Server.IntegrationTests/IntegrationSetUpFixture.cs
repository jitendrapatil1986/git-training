using NPoco;
using NServiceBus;
using NUnit.Framework;
using StructureMap;
using Warranty.Server.IntegrationTests.SetUp;

namespace Warranty.Server.IntegrationTests
{
    [SetUpFixture]
    public class IntegrationSetUpFixture
    {
        public IntegrationSetUpFixture()
        {
            TestIoC.Container = new Container(cfg => cfg.AddRegistry<WarrantyRegistry>());
            TestIoC.Container.Configure(cfg =>
            {

                cfg.Scan(scan =>
                {
                    scan.WithDefaultConventions();
                    scan.AssemblyContainingType<IDatabase>();
                    scan.AssemblyContainingType<WarrantyRegistry>();
                    scan.TheCallingAssembly();

                    scan.AddAllTypesOf(typeof(IHandleMessages<>));
                    scan.AddAllTypesOf(typeof(EntityBuilder<>));

                    scan.ConnectImplementationsToTypesClosing(typeof(IEntityBuilder<>));
                });
            });
        }
    }
}