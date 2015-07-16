using System.Configuration;

namespace Warranty.Server
{
    using Core.DataAccess;
    using log4net.Config;
    using NServiceBus;

    public class EndpointConfig : IConfigureThisEndpoint, AsA_Publisher, IWantCustomInitialization
    {
        public void Init()
        {
            SetLoggingLibrary.Log4Net(() => XmlConfigurator.Configure());

            var container = StructureMapConfig.CreateContainer();
            DbFactory.Setup(container);

            Configure.With()
                .StructureMapBuilder(container)
                .DefiningDataBusPropertiesAs(t => t.Name.EndsWith("DataBus"))
                .FileShareDataBus(ConfigurationManager.AppSettings["NServiceBus.FileShareDataBus"]);
        }
    }
}