using System.Web.Mvc;

namespace Warranty.UI.Controllers
{
    using Warranty.Core;
    using Warranty.Core.Features.ManageLookups;

    public class AdminController : Controller
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public ActionResult ManageLookups()
        {
            var result = _mediator.Request(new ManageLookupsQuery());
            return View(result);
        }
    }
}
