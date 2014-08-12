using StructureMap;

namespace Warranty.Server.IntegrationTests
{
    public static class StructureMapConfig
    {
        public static Container CreateContainer()
        {
            return new Container(cfg => cfg.AddRegistry<WarrantyRegistry>());
        }
    }
}