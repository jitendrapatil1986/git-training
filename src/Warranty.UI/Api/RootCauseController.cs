namespace Warranty.UI.Api
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using Warranty.Core;
    using Warranty.Core.Features.EditServiceCallLineItem;

    public class RootCauseController : ApiController
    {
        private readonly IMediator _mediator;

        public RootCauseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public IEnumerable<System.Web.Mvc.SelectListItem> RootCauses(string problemCode)
        {

            return _mediator.Request(new RootCauseQuery {ProblemCode = problemCode});
        }
    }
}