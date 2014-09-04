namespace Warranty.Core.Features.ManageLookups
{
    using System.Linq;
    using System.Reflection;
    using Entities.Lookups;
    using Extensions;

    public class ManageLookupsQueryHandler : IQueryHandler<ManageLookupsQuery, ManageLookupsModel>
    {
        public ManageLookupsModel Handle(ManageLookupsQuery query)
        {
            var lookups = Assembly.GetAssembly(typeof(LookupEntity)).GetTypes().Where(x=>ReflectionExtensions.IsAssignableTo<LookupEntity>(x) && !x.IsAbstract);
            return new ManageLookupsModel
                {
                    LookupTypes = lookups.Select(x => x.Name),
                };
        }
    }
}