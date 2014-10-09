namespace Warranty.Server
{
    using Core.DataAccess;
    using NServiceBus;
    using Security;

    public class EndpointConfig : IConfigureThisEndpoint, AsA_Server, IWantCustomInitialization
    {
        public void Init()
        {
            var container = StructureMapConfig.CreateContainer();
            DbFactory.Setup(container, new WarrantyServerUserSession());

            Configure.With().StructureMapBuilder(container);
        }
    }
}
