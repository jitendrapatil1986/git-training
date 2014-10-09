namespace Warranty.Core
{
    using CsvBuilder;
    using DataAccess;
    using Entities;
    using NPoco;
    using NPoco.FluentMappings;
    using StructureMap.Configuration.DSL;

    public class WarrantyCoreRegistry : Registry
    {
        public WarrantyCoreRegistry()
        {
            Scan(x =>
                     {
                         x.AssemblyContainingType<IAuditableEntity>();
                         x.AddAllTypesOf<IMap>();
                         x.WithDefaultConventions();
                     });

            For<IDatabase>().Use(() => DbFactory.DatabaseFactory.GetDatabase());
        }
    }
}
