namespace Warranty.Server
{
    using Core.DataAccess;
    using NServiceBus;

    public class EndpointConfig : IConfigureThisEndpoint, AsA_Publisher, IWantCustomInitialization
    {
        public void Init()
        {
            var container = StructureMapConfig.CreateContainer();
            DbFactory.Setup(container);

            Configure.With().StructureMapBuilder(container);
        }
    }
}
