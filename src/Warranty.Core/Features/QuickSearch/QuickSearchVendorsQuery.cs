namespace Warranty.Core.Features.QuickSearch
{
    using System.Collections.Generic;

    public class QuickSearchVendorsQuery : IQuery<IEnumerable<QuickSearchCallVendorModel>>
    {
        public string Query { get; set; }
    }
}