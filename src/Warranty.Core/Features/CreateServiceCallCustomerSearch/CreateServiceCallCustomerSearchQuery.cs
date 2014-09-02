namespace Warranty.Core.Features.CreateServiceCallCustomerSearch
{
    using System.Collections.Generic;

    public class CreateServiceCallCustomerSearchQuery : IQuery<IEnumerable<CustomerSearchModel>>
    {
        public string Query { get; set; }
    }
}
