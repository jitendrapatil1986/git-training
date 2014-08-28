namespace Warranty.UI.Api.Version1
{
    using System.Web.Mvc;
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
        public object SearchJobs(QuickSearchJobsQuery model)
        {
            var results = new[]
                              {
                                  new
                                      {
                                          id = 1,
                                          value = "Homeowner Name",
                                          jobnumber = "12345678",
                                          homeowner = "Homeowner",
                                          address = "1234 N. Main"
                                      },
                                  new
                                      {
                                          id = 2,
                                          value = "Homeowner Name 1",
                                          jobnumber = "87654321",
                                          homeowner = "Homeowner 1",
                                          address = "4321 N. Main"
                                      }
                              };
            return results;
        }

        [HttpGet]
        public object SearchCalls(QuickSearchCallsQuery model)
        {
            var results = _mediator.Request(model);
            return results;
        }
    }
}
