
using Common.Extensions;
using Common.Messages;
using log4net;
using log4net.Config;
using Warranty.HealthCheck.Config;

namespace Warranty.HealthCheck
{
    using NServiceBus;

	public class EndpointConfig : IConfigureThisEndpoint, AsA_Server, IWantCustomInitialization, IWantCustomLogging
    {
	    public void Init()
	    {
	        Configure.With()
                .StructureMapBuilder(StructuremapConfig.Init())
                .UseNHibernateSagaPersister()
                .UseNHibernateTimeoutPersister()
                .DefiningMessagesAs(t => (t.IsAssignableTo<IMessage>() && !(t.IsAssignableTo<ICommand>() || t.IsAssignableTo<IEvent>())) || t.IsBusMessage())
                .DefiningCommandsAs(t => t.IsAssignableTo<ICommand>() || t.IsBusCommand())
                .DefiningEventsAs(t => t.IsAssignableTo<IEvent>() || t.IsBusEvent());

            Configure.Features.Enable<NServiceBus.Features.Sagas>();
        }

        void IWantCustomLogging.Init()
        {
            SetLoggingLibrary.Log4Net(() => XmlConfigurator.Configure());
            LogManager.GetLogger(GetType()).Info("Logging Initialized");
        }
    }
}
