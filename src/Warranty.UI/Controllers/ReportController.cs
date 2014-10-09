using System.Collections.Generic;
using System.Web.Http;

namespace Warranty.UI.Controllers
{
    using System.Web.Mvc;
    using Warranty.Core;
    using Warranty.Core.Features.Report.WSRLoadingReport;

    public class ReportController : Controller
    {
        private readonly IMediator _mediator;

        public ReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public ActionResult WSRLoadingReport()
        {
            var resultModel = _mediator.Request(new WSRLoadingReportQuery());

            return View(resultModel);
        }

        [HttpPost]
        public ActionResult WSRLoadingReport(WSRLoadingReportModel model)
        {
            var resultModel = _mediator.Request(new WSRLoadingReportQuery { queryModel = model });

            return View(resultModel);
        }
    }
}