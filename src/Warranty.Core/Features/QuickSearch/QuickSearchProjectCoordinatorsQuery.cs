namespace Warranty.Core.Features.QuickSearch
{
    using System.Collections.Generic;

    public class QuickSearchProjectCoordinatorsQuery : IQuery<IEnumerable<QuickSearchProjectCoordinatorModel>>
    {
        public string Query { get; set; }
    }
}
