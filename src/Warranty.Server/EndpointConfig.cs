namespace Warranty.Server
{
    using System.Configuration;
    using System.Linq;
    using System.Reflection;
    using Core.DataAccess;
    using NServiceBus;
    using Security;
    using StructureMap;

    public class EndpointConfig : IConfigureThisEndpoint, AsA_Publisher, IWantCustomInitialization
    {
        public void Init()
        {
            var dataBusPath = ConfigurationManager.AppSettings["DataBusSharePath"];

            var container = new Container(x => x.AddRegistry<WarrantyRegistry>());
            DbFactory.Setup(container, new WarrantyServerUserSession());
            var assemblies = GetType().Assembly
                                      .GetReferencedAssemblies()
                                      .Select(Assembly.Load).ToList();
            assemblies.Add(GetType().Assembly);

            Configure.Transactions.Enable();

            Configure.With(assemblies)
                .StructureMapBuilder(container)
                .FileShareDataBus(dataBusPath)
                .EnablePerformanceCounters();
        }
    }
}
