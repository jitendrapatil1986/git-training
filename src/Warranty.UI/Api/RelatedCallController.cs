namespace Warranty.UI.Api
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using Warranty.Core;
    using Warranty.Core.Features.CreateServiceCall;

    public class RelatedCallController : ApiController
    {
        private readonly IMediator _mediator;

        public RelatedCallController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public IEnumerable<RelatedProblemCode> RelatedCalls(Guid jobId, string problemCode)
        {
            var results = _mediator.Request(new RelatedProblemCodeQuery { JobId=jobId, ProblemCode=problemCode });

            return results;
        }
    }
}
