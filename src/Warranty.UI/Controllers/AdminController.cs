using System.Web.Mvc;
using Warranty.Core;
using Warranty.Core.Features.AmountSpentWidget;

namespace Warranty.UI.Controllers
{
    public class AdminController : Controller
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public ActionResult Index()
        {
            var model = _mediator.Request(new AssignWSRsQuery());
            return View(model);
        }
    }
}
