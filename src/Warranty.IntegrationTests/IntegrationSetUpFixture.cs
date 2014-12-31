namespace Warranty.IntegrationTests
{
    using System.Configuration;
    using Accounting.Client;
    using Core.DataAccess;
    using Core.Extensions;
    using Core.Security;
    using MediatorMessagingTests;
    using MediatorMessagingTests.EntityBuilders;
    using NPoco;
    using NPoco.FluentMappings;
    using NUnit.Framework;
    using StructureMap;
    using Core;
    using Tests.Core;

    [SetUpFixture]
    public class IntegrationSetUpFixture
    {
        public IntegrationSetUpFixture()
        {
            ObjectFactory.Initialize(x =>
                                         {
                                             x.AddRegistry<WarrantyCoreRegistry>();
                                             x.AddRegistry<WarrantyCoreRegistry>();
                                             x.Scan(scan =>
                                                        {
                                                            scan.WithDefaultConventions();
                                                            scan.AssemblyContainingType<IDatabase>();
                                                            scan.AssemblyContainingType<ServiceCallEntityBuilder>();
                                                            scan.TheCallingAssembly();
                                                            scan.AddAllTypesOf(typeof (IMap));
                                                            scan.AddAllTypesOf(typeof (EntityBuilder<>));
                                                            scan.ConnectImplementationsToTypesClosing(typeof (IEntityBuilder<>));
                                                            scan.AddAllTypesOf(typeof(IAccountingClient));
                                                        });
                                             x.For<IUserSession>().Use<TestWarrantyUserSession>();

                                             var baseAccountingApiUri = ConfigurationManager.AppSettings["Accounting.API.BaseUri"];
                                             var timeoutInMilliseconds = ConfigurationManager.AppSettings["Accounting.API.TimeoutInMilliseconds"];
                                             var timeout = timeoutInMilliseconds.TryParseNullable();

                                             x.For<AccountingClientConfiguration>()
                                                 .Singleton()
                                                 .Use(() => new AccountingClientConfiguration(baseAccountingApiUri, timeout));
                                         });

            DbFactory.Setup(ObjectFactory.Container);
        }
        
    }
}