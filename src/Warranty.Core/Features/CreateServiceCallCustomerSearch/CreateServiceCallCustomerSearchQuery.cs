namespace Warranty.Core.Features.CreateServiceCallCustomerSearch
{
    public class CreateServiceCallCustomerSearchQuery : IQuery<CreateServiceCallCustomerSearchModel>
    {
        public string SearchCriteria { get; set; }
    }
}
