namespace Warranty.Core
{
    using DataAccess;
    using NPoco;
    using StructureMap.Configuration.DSL;

    public class WarrantyCoreRegistry : Registry
    {
        public WarrantyCoreRegistry()
        {
            For<IDatabase>().Use(() => DbFactory.DatabaseFactory.GetDatabase());
        }
    }
}
