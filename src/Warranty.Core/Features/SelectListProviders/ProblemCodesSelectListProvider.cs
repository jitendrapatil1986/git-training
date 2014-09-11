namespace Warranty.Core.Features.SelectListProviders
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Core;
    using ProblemCodes;

    public class ProblemCodesSelectListProvider : ISelectListProvider
    {
        private readonly IMediator _mediator;

        public ProblemCodesSelectListProvider(IMediator mediator)
        {
            _mediator = mediator;
        }

        public IEnumerable<SelectListItem> Provide()
        {
            var result = _mediator.Request(new ProblemCodesQuery());
            return result.Select(problemCode => new SelectListItem { Text = problemCode.DisplayName, Value = problemCode.Id.ToString() }).ToList();
        }
    }
}