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
        public IEnumerable<ManageLookupItemsModel> LookupSubtableDetails(string query)
        {
            var results = _mediator.Request(new ManageLookupItemsQuery {Query = query});

            return results;
        }

        [HttpPost]
        public bool DeleteLookup(DeleteLookupItemModel model)
        {
            var result = _mediator.Send(new DeleteLookupItemCommand {Id = model.Id, LookupType = model.LookupType});
            return result;
        }

        [HttpPost]
        public int CreateLookup(CreateLookupItemModel model)
        {
            var result = _mediator.Send(new CreateLookupItemCommand {DisplayName = model.DisplayName, LookupType = model.LookupType});
            return result;
        }
    }
}