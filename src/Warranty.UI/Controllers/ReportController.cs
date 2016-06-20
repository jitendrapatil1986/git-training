using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using Common.Security.Session;
using Warranty.Core.Features.MyDivisions;
using Warranty.Core.Features.MyProjects;
using Warranty.Core.Features.MyTeam;
using Warranty.Core.Features.Report.WSROpenActivity;

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
        private readonly IMapper _mapper;
        private IUser _currentUser;

        public ReportController(IMediator mediator, IMapper mapper, IUserSession currentSession)
        {
            _mediator = mediator;
            _mapper = mapper;
            _currentUser = currentSession.GetCurrentUser();
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

        public ActionResult WSROutstandingActivityReport()
        {
            var model = new WSROpenActivityModel(_currentUser);

            if (!model.ShowReportFilters)
                return WSROutstandingActivityReport(model);

            model.MyDivisions = _mapper.Map<List<SelectListItem>>(_mediator.Request(new MyDivisionsQuery()) ?? new Dictionary<Guid, string>());
            model.MyProjects = _mapper.Map<List<SelectListItem>>(_mediator.Request(new MyProjectsQuery()) ?? new Dictionary<Guid, string>());
            model.MyTeamMembers =_mapper.Map<List<SelectListItem>>(_mediator.Request(new MyTeamQuery()) ?? new List<MyTeamModel>());

            return View(model);
        }

        [HttpPost]
        public ActionResult WSROutstandingActivityReport(WSROpenActivityModel model)
        {
            var report = _mediator.Request(new WSROpenActivityQuery(model));

            report.MyDivisions = _mapper.Map<List<SelectListItem>>(_mediator.Request(new MyDivisionsQuery()) ?? new Dictionary<Guid, string>());
            report.MyProjects = _mapper.Map<List<SelectListItem>>(_mediator.Request(new MyProjectsQuery()) ?? new Dictionary<Guid, string>());
            report.MyTeamMembers = _mapper.Map<List<SelectListItem>>(_mediator.Request(new MyTeamQuery()) ?? new List<MyTeamModel>());

            // Setup for exporting
            //if(model.Action == "web")
            //    return View(report);

            //if(model.Action == "xls")
            //    return View(report);

            return View(report);
        }

    }
}
