namespace Warranty.Server
{
    using StructureMap;

    public static class StructureMapConfig
    {
        public static Container CreateContainer()
        {
            return new Container(cfg => cfg.AddRegistry<WarrantyRegistry>());
        }
    }
}