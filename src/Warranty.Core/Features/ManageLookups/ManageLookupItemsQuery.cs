using System.Collections.Generic;

namespace Warranty.Core.Features.ManageLookups
{
    public class ManageLookupItemsQuery : IQuery<IEnumerable<ManageLookupItemsModel>>
    {
        public string Query { get; set; }
    }
}
