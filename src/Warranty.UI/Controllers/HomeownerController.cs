using System.Web.Mvc;
using Warranty.Core;

namespace Warranty.UI.Controllers
{
    using Warranty.Core.Features.Homeowner;

    public class HomeownerController : Controller
    {
        private readonly IMediator _mediator;

        public HomeownerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public ActionResult AddOrUpdatePhoneNumber(AddOrUpdatePhoneNumberCommand command)
        {
            _mediator.Send(command);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddOrUpdateEmail(AddOrUpdateEmailCommand command)
        {
            _mediator.Send(command);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}
