namespace Warranty.Server
{
    using StructureMap.Configuration.DSL;

    public class WarrantyRegistry : Registry
    {
        public WarrantyRegistry()
        {
            Scan(scanner =>
            {
                scanner.WithDefaultConventions();
                
                scanner.TheCallingAssembly();
            });
        }
    }
}