namespace Warranty.UI.Api
{
    using System.Collections.Generic;
    using System.Web.Http;
    using Warranty.Core;
    using Warranty.Core.Features.ManageLookups;

    public class ManageLookupsController : ApiController
    {
        private readonly IMediator _mediator;

        public ManageLookupsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public IEnumerable<ManageLookupSubtableDetailsModel> LookupSubtableDetails(string query)
        {
            var results = _mediator.Request(new ManageLookupSubtableDetailsQuery {Query = query});

            return results;
        }

        [HttpPost]
        public int DeleteLookup(DeleteLookupSubtableDetailModel model)
        {
            var id = _mediator.Send(model);

            return id;
        }

        [HttpPost]
        public int CreateLookup(CreateLookupSubtableDetailModel model)
        {
            var id = _mediator.Send(model);

            return id;
        }
    }
}