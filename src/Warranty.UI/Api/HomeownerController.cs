namespace Warranty.UI.Api
{
    using System;
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

        public ActionResult AddOrUpdateAdditionalContact(AddOrUpdateAdditionalContactCommand command)
        {
            var id =_mediator.Send(command);
            return Json(new { success = true, homeownerContactId = id, isNew = command.HomeownerContactId == Guid.Empty, homeOwnercontactTypeVlue = command.HomeownerContactTypeValue }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteAdditionalContact(ServiceCallDeleteAdditionalContactCommand model)
        {
            _mediator.Send(model);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

    }
}
