namespace Warranty.UI.Controllers
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using System.Web;
    using System.Web.Mvc;
    using Core.Helpers;
    using Newtonsoft.Json.Linq;
    using Warranty.Core;
    using Warranty.Core.Extensions;
    using Warranty.Core.Features.AmountSpentWidget;
    using Warranty.Core.Features.AverageDaysClosedWidget;
    using Warranty.Core.Features.MyTeam;
    using Warranty.Core.Features.PercentClosedWithinSevenDaysWidget;
    using Warranty.Core.Features.PercentSurveyOutstandingWidget;
    using Warranty.Core.Features.PercentSurveyRecommendWidget;
    using Warranty.Core.Features.ToDoWidget;
    using Warranty.Core.Features.MyServiceTeamWidget;
    using Warranty.Core.Features.SendFeedback;
    using Warranty.Core.Features.ServiceCallsWidget;
    using Warranty.Core.Features.WarrantyDollarsSpentWidget;
    using Warranty.Core.Features.SetDefaultWidgetSize;

    public class HomeController : Controller
    {
        private readonly IMediator _mediator;

        public HomeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private readonly ClientApiHelper _clientApiHelper = new ClientApiHelper();
        private static object _clientApiUrls;

        private object BuildClientApiUrls()
        {
            return new JObject(
                new JProperty("Root", Url.Content("~")),
                from url in _clientApiHelper.GetUrls()
                select new JProperty(url.Name,
                    new JObject(
                        from method in url.Methods
                        select new JProperty(method.Action, method.IsWebApi ? Url.HttpRouteUrl("DefaultApi", new { controller = method.Controller, action = method.Action }) :  Url.Action(method.Action, method.Controller)
                    ))));
        }

        public string GetApiUrls()
        {
            _clientApiUrls = _clientApiUrls ?? BuildClientApiUrls();
            return _clientApiUrls.ToJson();
        }

        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult MyTeamWidget()
        {
            var model = _mediator.Request(new MyServiceTeamWidgetQuery());
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult MyTeamDropdown()
        {
            var model = _mediator.Request(new MyTeamQuery());
            return PartialView("_MyTeamDropdown", model);
        }

        [ChildActionOnly]
        public ActionResult ServiceCallsWidget()
        {
            var model = _mediator.Request(new ServiceCallsWidgetQuery());
            return PartialView(model);
        }
        
        public JsonResult SetDefaultWidgetSize(string defaultWidgetSize)
        {
            _mediator.Send(new SetDefaultWidgetSizeCommand { ServiceCallWidgetSize = Convert.ToInt32(defaultWidgetSize) });           
            return Json(new { success = "true" }, JsonRequestBehavior.AllowGet);            
        }

        [ChildActionOnly]
        public ActionResult WarrantyServiceRepServiceCallsWidget()
        {
            var model = _mediator.Request(new WarrantyServiceRepServiceCallsWidgetQuery());
            return PartialView("ServiceCallsWidget", model);
        }

        [ChildActionOnly]
        public ActionResult AmountSpentWidget()
        {
            var model = _mediator.Request(new AmountSpentWidgetQuery());
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult WarrantyDollarsSpentWidget()
        {
            var model = _mediator.Request(new WarrantyDollarsSpentWidgetQuery());
            return PartialView("_WarrantyDollarsSpentWidget", model);
        }

        [ChildActionOnly]
        public ActionResult AverageDaysClosedWidget()
        {
            var model = _mediator.Request(new AverageDaysClosedWidgetQuery());
            return PartialView("_AverageDaysClosedWidget", model);
        }

        [ChildActionOnly]
        public ActionResult PercentClosedWithinSevenDaysWidget()
        {
            var model = _mediator.Request(new PercentClosedWithinSevenDaysWidgetQuery());
            return PartialView("_PercentClosedWithinSevenDaysWidget", model);
        }

        [ChildActionOnly]
        public ActionResult WarrantyDollarsSpentWidgetWSR()
        {
            var model = _mediator.Request(new WarrantyDollarsSpentWidgetWSRQuery());
            return PartialView("_WarrantyDollarsSpentWidget", model);
        }

        [ChildActionOnly]
        public ActionResult AverageDaysClosedWidgetWSR()
        {
            var model = _mediator.Request(new AverageDaysClosedWidgetWSRQuery());
            return PartialView("_AverageDaysClosedWidget", model);
        }

        [ChildActionOnly]
        public ActionResult PercentClosedWithinSevenDaysWidgetWSR()
        {
            var model = _mediator.Request(new PercentClosedWithinSevenDaysWidgetWSRQuery());
            return PartialView("_PercentClosedWithinSevenDaysWidget", model);
        }

        [ChildActionOnly]
        public ActionResult PercentSurveyRecommendWidget()
        {
            var model = _mediator.Request(new PercentSurveyRecommendWidgetQuery());
            return PartialView("_PercentSurveyRecommendWidget", model);
        }

        [ChildActionOnly]
        public ActionResult PercentSurveyRecommendWidgetWSR()
        {
            var model = _mediator.Request(new PercentSurveyRecommendWidgetWSRQuery());
            return PartialView("_PercentSurveyRecommendWidget", model);
        }

        [ChildActionOnly]
        public ActionResult PercentSurveyOutstandingWidget()
        {
            var model = _mediator.Request(new PercentSurveyOutstandingWidgetQuery());
            return PartialView("_PercentSurveyOutstandingWidget", model);
        }

        [ChildActionOnly]
        public ActionResult PercentSurveyOutstandingWidgetWSR()
        {
            var model = _mediator.Request(new PercentSurveyOutstandingWidgetWSRQuery());
            return PartialView("_PercentSurveyOutstandingWidget", model);
        }
        
        [ChildActionOnly]
        public ActionResult ToDoWidget()
        {
            var model = _mediator.Request(new ToDoWidgetQuery());
            return PartialView(model);
        }

        public ActionResult SaveLastSelectedToDoFilter(string value)
        {
            _mediator.Send(new ToDoLastSelectedFilterSaveCommand
                {
                    LastSelectedFilter = value
                });
            return Json(new { success = "true" }, JsonRequestBehavior.AllowGet);
        }

        [ChildActionOnly]
        public string GetRequireJsUrl()
        {
            var requirePath = VirtualPathUtility.ToAbsolute("~/Scripts/lib/require.js");
            var mainPath = VirtualPathUtility.ToAbsolute("~/Scripts/app/main.js");
            return string.Format("<script data-main='{0}' src='{1}'></script>", mainPath , requirePath);
        }

        [ChildActionOnly]
        public string GetPageRequireJs(ViewContext context)
        {
            var view = context.View as RazorView;
            var values = context.RouteData.Values;
            var controller = values["controller"].ToString().ToLower();
            var viewName = Path.GetFileNameWithoutExtension(view.ViewPath);
            var loc = string.Format("~/Scripts/app/{0}/{1}.js", controller, viewName);
            var script = "";

            if (System.IO.File.Exists(Server.MapPath(loc)))
            {
                script = string.Format("<script src='{0}?_={1}'></script>", VirtualPathUtility.ToAbsolute(loc), VersionHelper.ApplicationVersionNumber());
            }
            return script;
        }

        public JsonResult SendFeedback(string subject, string body, string uiversion)
        {
            var request = HttpContext.Request;
            var urlReferrer = request.UrlReferrer;
            var browser = request.Browser;
            var userAgent = request.UserAgent;
            var hostAddress = request.UserHostAddress;
            var hostName = request.UserHostName;

            var mailMessage = new MailMessage
            {
                Subject = subject,
                Body = "Message: " + body
                + "\n\n\nUser: " + User.Identity.Name
                + "\nURL: " + urlReferrer
                + "\nUI Version: " + uiversion
                + "\nBrowser: " + browser.Browser + " " + (browser.Version ?? "")
                + "\nUser Agent: " + userAgent
                + "\nHost Address: " + hostAddress
                + "\nHost Name: " + hostName
            };

            var result = new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            mailMessage.To.Add(ConfigurationManager.AppSettings["sendFeedbackAddresses"]);

            try
            {
                _mediator.Send(new SendFeedbackCommand {MailMessage = mailMessage});
                result.Data = "success";
            }
            catch (Exception)
            {
                result.Data = "error";
            }

            return result;
        }
    }
}
