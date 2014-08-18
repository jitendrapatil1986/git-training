using Warranty.Core.Features.AmountSpentWidget;

namespace Warranty.UI.Controllers
{
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using Warranty.Core;
    using Warranty.Core.Features.ServiceCallsWidget;

    public class HomeController : Controller
    {
        private readonly IMediator _mediator;

        public HomeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public ActionResult Index()
        {
            return View();
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
