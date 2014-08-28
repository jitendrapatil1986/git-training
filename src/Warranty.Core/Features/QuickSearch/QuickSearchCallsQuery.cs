namespace Warranty.Core.Features.QuickSearch
{
    public class QuickSearchCallsQuery : IQuery<SearchResults>
    {
        public string Query { get; set; }
        public bool IncludeInactive { get; set; }
    }
}
