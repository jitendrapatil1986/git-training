namespace Warranty.Core.Features.ManageLookups
{
    using System.Collections.Generic;
    using System.Linq;
    using Entities.Lookups;
    using NPoco;

    public class ManageLookupSubtableDetailsQueryHandler : IQueryHandler<ManageLookupSubtableDetailsQuery, IEnumerable<ManageLookupSubtableDetailsModel>>
    {
        private readonly IDatabase _database;

        public ManageLookupSubtableDetailsQueryHandler(IDatabase database)
        {
            _database = database;
        }

        public IEnumerable<ManageLookupSubtableDetailsModel> Handle(ManageLookupSubtableDetailsQuery query)
        {
            var lookupType = LookupEntity.GetTypeFromName(query.Query);
            var fetchMethod = typeof (Database).GetMethods().Single(x=>x.Name == "Fetch" && !x.GetParameters().Any());
            var genericFetchMethod = fetchMethod.MakeGenericMethod(lookupType);
            
            var result = genericFetchMethod.Invoke(_database, null) as IEnumerable<LookupEntity>;

            return result.Select(x => new ManageLookupSubtableDetailsModel {DisplayName = x.DisplayName, Id = x.Id});
        }
    }
}
