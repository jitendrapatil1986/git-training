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
    using Warranty.Core.Features.PercentClosedWithinSevenDaysWidget;
    using Warranty.Core.Features.ToDoWidget;
    using Warranty.Core.Features.MyServiceTeamWidget;
    using Warranty.Core.Features.SendFeedback;
    using Warranty.Core.Features.ServiceCallsWidget;
    using Warranty.Core.Features.WarrantyDollarsSpentWidget;

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
        public ActionResult ServiceCallsWidget()
        {
            var model = _mediator.Request(new ServiceCallsWidgetQuery());
            return PartialView(model);
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
        public ActionResult ToDoWidget()
        {
            var model = _mediator.Request(new ToDoWidgetQuery());
            return PartialView(model);
        }

        [ChildActionOnly]
        public string GetRequireJSUrl(ViewContext context)
        {
            var view = context.View as RazorView;
            var values = context.RouteData.Values;
            var controller = values["controller"].ToString().ToLower();
            var viewName = Path.GetFileNameWithoutExtension(view.ViewPath);
            var requirePath = VirtualPathUtility.ToAbsolute("~/Scripts/lib/require.js");

            var loc = string.Format("~/Scripts/app/{0}/{1}.js", controller, viewName);

            if (System.IO.File.Exists(Server.MapPath(loc)))
            {
                return "<script data-main='" + VirtualPathUtility.ToAbsolute(loc) + "' src='" + requirePath + "'></script>";
            }

            return "<script data-main='" + VirtualPathUtility.ToAbsolute("~/Scripts/app/Shared/Default.js") + "' src='" + requirePath + "'></script>";
        }

        public JsonResult SendFeedback(string subject, string body)
        {
            var request = HttpContext.Request;
            var urlReferrer = request.UrlReferrer;
            var browser = request.Browser.Browser;
            var userAgent = request.UserAgent;
            var hostAddress = request.UserHostAddress;
            var hostName = request.UserHostName;

            var mailMessage = new MailMessage
            {
                Subject = subject,
                Body = "Message: " + body
                + "\n\n\nUser: " + User.Identity.Name
                + "\nURL: " + urlReferrer
                + "\nBrowser: " + browser
                + "\nUser Agent: " + userAgent
                + "\nHost Address: " + hostAddress
                + "\nHost Name: " + hostName
            };

            var result = new JsonResult();

            mailMessage.To.Add(ConfigurationManager.AppSettings["sendFeedbackAddresses"]);

            try
            {
                _mediator.Send(new SendFeedbackCommand {MailMessage = mailMessage});
            }
            catch (Exception)
            {
                result.Data = "error";
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }

            result.Data = "success";
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            return result;
        }
    }
}
