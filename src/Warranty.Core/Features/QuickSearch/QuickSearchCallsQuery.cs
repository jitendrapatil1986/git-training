namespace Warranty.Core.Features.QuickSearch
{
    using System.Collections.Generic;

    public class QuickSearchCallsQuery : IQuery<IEnumerable<QuickSearchCallModel>>
    {
        public string Query { get; set; }
        public bool IncludeInactive { get; set; }
    }
}
