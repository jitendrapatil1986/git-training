namespace Warranty.UI.Api
{
    using System.Collections.Generic;
    using System.Web.Http;
    using Warranty.Core;
    using Warranty.Core.Features.QuickSearch;

    public class QuickSearchController : ApiController
    {
        private readonly IMediator _mediator;

        public QuickSearchController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpGet]
        public IEnumerable<QuickSearchJobModel> SearchJobs(string query, bool includeInactive = false)
        {
            var results = _mediator.Request(new QuickSearchJobsQuery{Query = query, IncludeInactive = includeInactive});
            return results;
        }

        [HttpGet]
        public IEnumerable<QuickSearchCallModel> SearchCalls(string query, bool includeInactive = false)
        {
            var results = _mediator.Request(new QuickSearchCallsQuery {Query = query, IncludeInactive = includeInactive});
            return results;
        }
    }
}
