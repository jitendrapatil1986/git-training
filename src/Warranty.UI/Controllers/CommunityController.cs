using System.Web.Mvc;
using Warranty.Core;
using Warranty.Core.Features.CommunityEmployeeAssignment;

namespace Warranty.UI.Controllers
{
    public class CommunityController : Controller
    {
        private readonly IMediator _mediator;

        public CommunityController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public ActionResult AssignEmployee(CommunityEmployeeAssignmentCommand command)
        {
            var result  = _mediator.Send(command);

            return Json(new { success = result }, JsonRequestBehavior.AllowGet);
        }
    }
}
