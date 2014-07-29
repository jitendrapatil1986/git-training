using NServiceBus;

namespace Warranty.Server
{
	public class EndpointConfig : IConfigureThisEndpoint, AsA_Server, IWantCustomInitialization
    {
	    public void Init()
	    {
	        var container = StructureMapConfig.CreateContainer();

	        Configure.With().StructureMapBuilder(container);
	    }
    }
}
