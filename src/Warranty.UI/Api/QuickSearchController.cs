namespace Warranty.UI.Api
{
    using System.Collections.Generic;
    using System.Web.Http;
    using Warranty.Core;
    using Warranty.Core.Features.CreateServiceCallCustomerSearch;
    using Warranty.Core.Features.QuickSearch;
    using Common.Security.Session;

    public class QuickSearchController : ApiController
    {
        private readonly IMediator _mediator;
        private readonly IUser _currentUser;

        public QuickSearchController(IMediator mediator, IUserSession currentSession)
        {
            _mediator = mediator;
            _currentUser = currentSession.GetCurrentUser();
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
        public IEnumerable<QuickSearchEmployeeModel> Employees(string query)
        {
            var results = _mediator.Request(new QuickSearchEmployeesQuery {Query = query});
            return results;
        }

        [HttpGet]
        public IEnumerable<QuickSearchCallVendorModel> Vendors(string query, string cityCode, string invoicePayableCode)
        {
            var userMarkets = _currentUser.Markets;
            var userMarketsString = string.Join(",", userMarkets);
            var markets = cityCode ?? userMarketsString;
            var results = _mediator.Request(new QuickSearchVendorsQuery
                {
                    Query = query,
                    CityCode = markets,
                    InvoicePayableCode = invoicePayableCode
                });
            return results;
        }

        [HttpGet]
        public IEnumerable<QuickSearchDivisionOrProjectCoordinatorModel> ProjectCoordinators(string query)
        {
            var results = _mediator.Request(new QuickSearchDivisionOrProjectCoordinatorsQuery { Query = query});
            return results;
        }
    }
}
