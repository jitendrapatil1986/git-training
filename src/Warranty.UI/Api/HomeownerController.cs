namespace Warranty.UI.Api
{
    using System.Web.Mvc;
    using Warranty.Core;
    using Warranty.Core.Features;
    using Warranty.Core.Features.Homeowner;

    public class HomeownerController : Controller
    {
        private readonly IMediator _mediator;

        public HomeownerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public ActionResult AddOrUpdatePhoneNumber(AddOrUpdatePhoneNumberCommand command)
        {
            _mediator.Send(command);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddOrUpdateEmail(AddOrUpdateEmailCommand command)
        {
            _mediator.Send(command);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveAdditionalContacts(AdditionalContactsModel command)
        {
            _mediator.Send(command);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

    }
}
