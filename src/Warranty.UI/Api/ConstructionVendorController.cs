namespace Warranty.UI.Api
{
    using System.Collections.Generic;
    using System.Web.Http;
    using Warranty.Core;
    using Warranty.Core.Features.ServiceCallSummary.ServiceCallLineItem;

    public class ConstructionVendorController : ApiController
    {
        private readonly IMediator _mediator;

        public ConstructionVendorController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public IEnumerable<ConstructionVendorModel> ConstructionVendors(string jobNumber, string costCode)
        {
            return _mediator.Request<IEnumerable<ConstructionVendorModel>>(new ConstructionVendorQuery() { JobNumber = jobNumber, CostCode = costCode });
        }
    }
}