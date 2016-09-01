namespace Warranty.Core.Features.QuickSearch
{
    using System.Collections.Generic;

    public class QuickSearchDivisionOrProjectCoordinatorsQuery : IQuery<IEnumerable<QuickSearchDivisionOrProjectCoordinatorModel>>
    {
        public string Query { get; set; }
    }
}
