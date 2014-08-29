namespace Warranty.Core.Features.QuickSearch
{
    using System.Collections.Generic;

    public class QuickSearchJobsQuery : IQuery<IEnumerable<QuickSearchJobModel>>
    {
        public string Query { get; set; }
        public bool IncludeInactive { get; set; }
    }
}
