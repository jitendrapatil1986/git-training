namespace Warranty.UI.Api
{
    using System.Collections.Generic;
    using System.Web.Http;
    using Warranty.Core;
    using Warranty.Core.Features.EditServiceCallLineItem;

    public class ProblemDetailController : ApiController
    {
        private readonly IMediator _mediator;

        public ProblemDetailController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public IEnumerable<System.Web.Mvc.SelectListItem> ProblemDetails(string problemJdeCode)
        {

            return _mediator.Request(new ProblemDetailQuery { ProblemJdeCode = problemJdeCode });
        }
    }
}