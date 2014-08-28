namespace Warranty.Core.Features.QuickSearch
{
    public class QuickSearchJobsQuery : IQuery<SearchResults>
    {
        public string Query { get; set; }
        public bool IncludeInactive { get; set; }
    }
}
