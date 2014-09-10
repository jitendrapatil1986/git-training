namespace Warranty.Core.Features.ManageLookups
{
    using System.Collections.Generic;
    using System.Linq;
    using Entities.Lookups;
    using NPoco;

    public class ManageLookupItemsQueryHandler : IQueryHandler<ManageLookupItemsQuery, IEnumerable<ManageLookupItemsModel>>
    {
        private readonly IDatabase _database;

        public ManageLookupItemsQueryHandler(IDatabase database)
        {
            _database = database;
        }

        public IEnumerable<ManageLookupItemsModel> Handle(ManageLookupItemsQuery query)
        {
            var lookupType = LookupEntity.GetTypeFromName(query.Query);
            var fetchMethod = typeof (Database).GetMethods().Single(x=>x.Name == "Fetch" && !x.GetParameters().Any());
            var genericFetchMethod = fetchMethod.MakeGenericMethod(lookupType);
            
            var result = genericFetchMethod.Invoke(_database, null) as IEnumerable<LookupEntity>;

            return result.Select(x => new ManageLookupItemsModel {DisplayName = x.DisplayName, Id = x.Id}).OrderBy(x => x.DisplayName);
        }
    }
}
