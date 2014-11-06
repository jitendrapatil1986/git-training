namespace Warranty.UI.Api
{
    using System.Collections.Generic;
    using System.Web.Http;
    using Warranty.Core;
    using Warranty.Core.Features.CreateServiceCallCustomerSearch;
    using Warranty.Core.Features.QuickSearch;

    public class QuickSearchController : ApiController
    {
        private readonly IMediator _mediator;

        public QuickSearchController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpGet]
        public IEnumerable<QuickSearchJobModel> Jobs(string query)
        {
            var results = _mediator.Request(new QuickSearchJobsQuery{Query = query});
            return results;
        }

        [HttpGet]
        public IEnumerable<QuickSearchCallModel> Calls(string query)
        {
            var results = _mediator.Request(new QuickSearchCallsQuery {Query = query});
            return results;
        }

        [HttpGet]
        public IEnumerable<CustomerSearchModel> Customers(string query)
        {
            var results = _mediator.Request(new CreateServiceCallCustomerSearchQuery {Query = query});
            return results;
        }

        [HttpGet]
        public IEnumerable<QuickSearchCallVendorModel> Vendors(string query)
        {
            var results = _mediator.Request(new QuickSearchVendorsQuery { Query = query });
            return results;
        }
    }
}
