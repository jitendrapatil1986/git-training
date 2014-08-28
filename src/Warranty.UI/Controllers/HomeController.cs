using Warranty.Core.Features.AmountSpentWidget;
using Warranty.Core.Features.ToDoWidget;

namespace Warranty.UI.Controllers
{
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Core.Helpers;
    using Newtonsoft.Json.Linq;
    using Warranty.Core;
    using Warranty.Core.Extensions;
    using Warranty.Core.Features.MyServiceTeamWidget;
    using Warranty.Core.Features.ServiceCallsWidget;

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
                        select new JProperty(method.Action, Url.Action(method.Controller, method.Action)
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
        public ActionResult AmountSpentWidget()
        {
            var model = _mediator.Request(new AmountSpentWidgetQuery());
            return PartialView(model);
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
    }
}
