using System.Configuration;
using Common.Extensions;
using Common.Messages;
using log4net.Config;
using NServiceBus;
using Warranty.Core.DataAccess;

namespace Warranty.Server
{
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
                .DefiningMessagesAs(t => (t.IsAssignableTo<IMessage>() && !(t.IsAssignableTo<ICommand>() || t.IsAssignableTo<IEvent>())) || t.IsBusMessage())
                .DefiningCommandsAs(t => t.IsAssignableTo<ICommand>() || t.IsBusCommand())
                .DefiningEventsAs(t => t.IsAssignableTo<IEvent>() || t.IsBusEvent())
                .FileShareDataBus(ConfigurationManager.AppSettings["NServiceBus.FileShareDataBus"]);

            Configure.Features.Enable<NServiceBus.Features.Sagas>();
        }
    }
}