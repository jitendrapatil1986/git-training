using NPoco;
using NServiceBus;
using NUnit.Framework;
using StructureMap;
using Warranty.Server.IntegrationTests.SetUp;

namespace Warranty.Server.IntegrationTests
{
    using Core;
    using Core.DataAccess;
    using NPoco.FluentMappings;
    using Server.Handlers;
    using Tests.Core;

    [SetUpFixture]
    public class IntegrationSetUpFixture
    {
        public IntegrationSetUpFixture()
        {
            ObjectFactory.Initialize(x =>
                                         {
                                             x.AddRegistry<WarrantyRegistry>();
                                             x.AddRegistry<WarrantyCoreRegistry>();
                                             x.Scan(scan =>
                                                        {
                                                            scan.WithDefaultConventions();
                                                            scan.AssemblyContainingType<IDatabase>();
                                                            scan.AssemblyContainingType<PaymentAddedHandler>();
                                                            scan.TheCallingAssembly();

                                                            scan.AddAllTypesOf(typeof (IMap));
                                                            scan.AddAllTypesOf(typeof (IHandleMessages<>));
                                                            scan.AddAllTypesOf(typeof (EntityBuilder<>));

                                                            scan.ConnectImplementationsToTypesClosing(typeof (IEntityBuilder<>));
                                                        });
                                         });

            DbFactory.Setup(ObjectFactory.Container, new TestWarrantyUserSession());
        }
    }
}