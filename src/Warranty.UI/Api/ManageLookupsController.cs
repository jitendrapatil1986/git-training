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
        public bool DeleteLookup(DeleteLookupSubtableDetailModel model)
        {
            var result = _mediator.Send(new DeleteLookupSubtableDetailCommand {Model = model});

            return result;
        }

        [HttpPost]
        public bool CreateLookup(CreateLookupSubtableDetailModel model)
        {
            var result = _mediator.Send(new CreateLookupSubtableDetailCommand {Model = model});

            return result;
        }
    }
}