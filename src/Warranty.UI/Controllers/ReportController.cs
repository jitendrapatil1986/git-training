using System;
using System.Web.Mvc;

namespace Warranty.UI.Controllers
{
    using Warranty.Core;
    using Warranty.Core.Features.WarrantyBonusSummary;

    public class ReportController : Controller
    {
        private readonly IMediator _mediator;

        public ReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public ActionResult WarrantyBonusSummaryWSRReport()
        {
            var resultModel = _mediator.Request(new WarrantyBonusSummaryWSRQuery());

            return View(resultModel);
        }

        [HttpPost]
        public ActionResult WarrantyBonusSummaryWSRReport(WarrantyBonusSummaryModel model)
        {
            var resultModel = _mediator.Request(new WarrantyBonusSummaryWSRQuery { queryModel = model });

            return View(resultModel);
        }

        public ActionResult WarrantyBonusSummary()
        {
            var model = _mediator.Request(new WarrantyBonusSummaryQuery());

            return View(model);
        }
    }
}