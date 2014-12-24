namespace Warranty.Core.Features.QuickSearch
{
    using System.Collections.Generic;

    public class QuickSearchEmployeesQuery : IQuery<IEnumerable<QuickSearchEmployeeModel>>
    {
        public string Query { get; set; }
    }
}