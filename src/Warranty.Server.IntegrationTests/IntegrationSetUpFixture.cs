using log4net;
using NPoco;
using NServiceBus;
using NUnit.Framework;
using StructureMap;
using Warranty.Server.IntegrationTests.SetUp;

namespace Warranty.Server.IntegrationTests
{
    using System.Configuration;
    using Accounting.Client;
    using Core;
    using Core.DataAccess;
    using Core.Extensions;
    using Common.Security.Session;
    using NPoco.FluentMappings;
    using Server.Handlers.Payments;
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
                                             x.For<IUserSession>().Use<TestWarrantyUserSession>();
                                             x.For<ILog>().AlwaysUnique().Use(c =>
                                             {
                                                 var parentType = c.ParentType ?? c.BuildStack.Current.ConcreteType;
                                                 return LogManager.GetLogger(parentType);
                                             });

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
