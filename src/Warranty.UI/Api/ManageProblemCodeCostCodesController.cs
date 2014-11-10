namespace Warranty.UI.Api
{
    using System.Collections.Generic;
    using System.Web.Http;
    using Warranty.Core;
    using Warranty.Core.Features.ManageProblemCodeCostCodes;
    using Warranty.Core.Features.ServiceCallSummary.ServiceCallLineItem;

    public class ManageProblemCodeCostCodesController : ApiController
    {
        private readonly IMediator _mediator;

        public ManageProblemCodeCostCodesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public IEnumerable<ProblemCodeCostCodeModel> ProblemCodeCostCodes(string cityCode)
        {
            return _mediator.Request<IEnumerable<ProblemCodeCostCodeModel>>(new ProblemCodeCostCodeQuery() { CityCode = cityCode });
        }

        [HttpPost]
        public bool UpdateProblemCodeCostCode(ProblemCodeCostCodeUpdateModel model)
        {
            var result =
                _mediator.Send(new ProblemCodeCostCodeUpdateCommand()
                {
                    CityCode = model.CityCode,
                    ProblemJdeCode = model.ProblemJdeCode,
                    CostCode = model.CostCode
                });
            return result;
        }
    }
}