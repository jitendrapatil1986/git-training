using System;
using NPoco;
using NServiceBus.MessageMutator;
using Warranty.Core.DataAccess;
using Warranty.Core.Security;
using Warranty.Server.Security;

namespace Warranty.Server
{
    using System.Configuration;
    using Accounting.Client;
    using Core.Entities;
    using Core.Extensions;
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
                scanner.AssemblyContainingType<IAccountingClient>();
                scanner.AddAllTypesOf((typeof(IAccountingClient)));
                scanner.AddAllTypesOf<IMap>();

                var loggingEnabled = ConfigurationManager.AppSettings["MessageLoggingEnabled"];
                if (loggingEnabled != null && loggingEnabled.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    For<IMutateIncomingMessages>().Singleton().Use<LoggingMutator>();
                    For<IMutateIncomingTransportMessages>().Singleton().Use<LoggingTransportMutator>();
                }

                For<IDatabase>()
                    .LifecycleIs(new ThreadLocalStorageLifecycle())
                    .Use(() => DbFactory.DatabaseFactory.GetDatabase());

                For<IUserSession>().Use<WarrantyServerUserSession>();

                var baseAccountingApiUri = ConfigurationManager.AppSettings["Accounting.API.BaseUri"];
                var timeoutInMilliseconds = ConfigurationManager.AppSettings["Accounting.API.TimeoutInMilliseconds"];
                var timeout = timeoutInMilliseconds.TryParseNullable();

                For<AccountingClientConfiguration>()
                    .Singleton()
                    .Use(() => new AccountingClientConfiguration(baseAccountingApiUri, timeout));
            });
        }
    }
}
