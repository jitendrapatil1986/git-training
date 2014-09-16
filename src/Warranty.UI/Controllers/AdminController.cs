using System.Web.Mvc;
using Warranty.Core;
using Warranty.Core.Features.AssignWSRs;
using Warranty.Core.Features.ManageLookups;
using Warranty.UI.Core.Filters;

namespace Warranty.UI.Controllers
{
    public class AdminController : Controller
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ModelStateToTempData]
        public ActionResult ManageAssignments()
        {
            var model = _mediator.Request(new AssignWSRsQuery());
            return View(model);
        }

        [HttpPost]
        [ModelStateToTempData]
        public ActionResult ManageAssignments(AssignWSRsModel newAddAssignment)
        {
            if (ModelState.IsValid)
            {
                _mediator.Send(new AssignWSRCommand
                {
                    CommunityId = newAddAssignment.SelectedCommunityId,
                    EmployeeId = newAddAssignment.SelectedEmployeeId
                });
            }

            return RedirectToAction("ManageAssignments");
        }

        public ActionResult ManageLookups()
        {
            var result = _mediator.Request(new ManageLookupsQuery());
            return View(result);
        }
    }
}
