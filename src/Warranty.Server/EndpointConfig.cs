namespace Warranty.Server
{
    using NServiceBus;

    public class EndpointConfig : IConfigureThisEndpoint, AsA_Server, IWantCustomInitialization
    {
        public void Init()
        {
            var container = StructureMapConfig.CreateContainer();

            Configure.With().StructureMapBuilder(container);
        }
    }
}
