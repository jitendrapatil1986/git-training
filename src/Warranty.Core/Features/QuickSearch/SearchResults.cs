namespace Warranty.Core.Features.QuickSearch
{
    using System.Collections.Generic;

    public class SearchResults
    {
        public SearchResults(IDictionary<string, string> data)
        {
            Data = data;
            ResultCount = data.Count;
        }

        public IDictionary<string, string> Data { get; private set; }

        public int ResultCount { get; private set; }

        public int TotalMatches { get; set; }

        public string Query { get; set; }
    }
}
