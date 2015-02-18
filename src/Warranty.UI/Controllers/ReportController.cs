using System;
using System.Web.Mvc;

namespace Warranty.UI.Controllers
{
    using Warranty.Core;
    using Warranty.Core.Features.Report.Achievement;
    using Warranty.Core.Features.Report.MailMerge;
    using Warranty.Core.Features.Report.Saltline;
    using Warranty.Core.Features.Report.WSRCallSummary;
    using Warranty.Core.Features.Report.WSRLoadingReport;
    using Warranty.Core.Features.Report.WSROpenedClosedCalls;
    using Warranty.Core.Features.Report.WSRSummary;
    using Warranty.Core.Features.Report.WarrantyBonusSummary;

    public class ReportController : Controller
    {
        private readonly IMediator _mediator;

        public ReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public ActionResult MailMerge()
        {
            var model = new MailMergeQuery();
            return View(model);
        }

        [HttpPost]
        public ActionResult MailMerge(MailMergeQuery query)
        {
            var model = _mediator.Request(query);
            return View(model);
        }

        [HttpPost]
        public ActionResult MailMergeDownloadAsCsv(DateTime date)
        {
            var query = new MailMergeQuery
                {
                    Date = date
                };
            var model = _mediator.Request(query);

            var result = _mediator.Request(new MailMergeDownloadAsCsvQuery
                {
                    ReportData = model.Result,
                    Date = date
                });

            return File(result.Bytes, result.MimeMapping, result.FileName);
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

        public ActionResult WarrantyBonusSummaryWSRReport()
        {
            var resultModel = _mediator.Request(new WarrantyBonusSummaryWSRQuery());

            return View(resultModel);
        }

        [HttpPost]
        public ActionResult WarrantyBonusSummaryWSRReport(WarrantyBonusSummaryModel model)
        {
            var resultModel = _mediator.Request(new WarrantyBonusSummaryWSRQuery { Model = model });

            return View(resultModel);
        }

        public ActionResult AchievementReport()
        {
            var resultModel = _mediator.Request(new AchievementReportQuery());

            return View(resultModel);
        }

        [HttpPost]
        public ActionResult AchievementReport(AchievementReportModel model)
        {
            var resultModel = _mediator.Request(new AchievementReportQuery { queryModel = model });

            return View(resultModel);
        }

        public ActionResult SaltlineReport()
        {
            var resultModel = _mediator.Request(new SaltlineReportQuery());

            return View(resultModel);
        }

        [HttpPost]
        public ActionResult SaltlineReport(SaltlineReportModel model)
        {
            var resultModel = _mediator.Request(new SaltlineReportQuery { queryModel = model });

            return View(resultModel);
        }

        public ActionResult WSRSummaryReport()
        {
            var resultModel = _mediator.Request(new WSRSummaryQuery());

            return View(resultModel);
        }

        public ActionResult WSRCallSummaryReport()
        {
            var resultModel = _mediator.Request(new WSRCallSummaryQuery());

            return View(resultModel);
        }

        [HttpPost]
        public ActionResult WSRCallSummaryReport(WSRCallSummaryModel model)
        {
            var resultModel = _mediator.Request(new WSRCallSummaryQuery() { queryModel = model });

            return View(resultModel);
        }

        public ActionResult WSROpenedClosedCallReport()
        {
            var resultModel = _mediator.Request(new WSROpenedClosedCallsQuery());

            return View(resultModel);
        }

        [HttpPost]
        public ActionResult WSROpenedClosedCallReport(WSROpenedClosedCallsModel model)
        {
            var resultModel = _mediator.Request(new WSROpenedClosedCallsQuery() { queryModel = model });

            return View(resultModel);
        }
    }
}
