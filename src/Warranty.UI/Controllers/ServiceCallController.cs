using Warranty.Core.Features.ServiceCallApproval;
using Warranty.Core.Features.ServiceCallToggleActions;
using Warranty.UI.Mailer;

namespace Warranty.UI.Controllers
{
    using System;
    using System.Web.Mvc;
    using Warranty.Core;
    using Warranty.Core.Entities;
    using Warranty.Core.Enumerations;
    using Warranty.Core.Features.CreateServiceCall;
    using Warranty.Core.Features.CreateServiceCallCustomerSearch;
    using Warranty.Core.Features.CreateServiceCallVerifyCustomer;
    using Warranty.Core.Features.ServiceCallSummary;
    using System.Linq;
    using Warranty.Core.Security;

    public class ServiceCallController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IWarrantyMailer _mailer;
        private readonly IUserSession _userSession;

        public ServiceCallController(IMediator mediator, IWarrantyMailer mailer, IUserSession userSession)
        {
            _mediator = mediator;
            _mailer = mailer;
            _userSession = userSession;
        }

        public ActionResult Reassign(Guid id)
        {
            return View();
        }

        public ActionResult Close(Guid id)
        {
            return View();
        }

        public ActionResult RequestPayment(Guid id)
        {
            return View();
        }

        public ActionResult CallSummary(Guid id)
        {
            var model = _mediator.Request(new ServiceCallSummaryQuery
                {
                    ServiceCallId = id
                });

            return View(model);
        }

        public JsonResult GetServiceLines(Guid id)
        {
            var model = _mediator.Request(new ServiceCallSummaryQuery
                {
                    ServiceCallId = id
                });

            return Json(model.ServiceCallLines,JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchCustomer(string searchCriteria)
        {
            var model = _mediator.Request(new CreateServiceCallCustomerSearchQuery
                {
                    Query = searchCriteria
                });

            return View(model);
        }

        public ActionResult VerifyCustomer(Guid id)
        {
            var model = _mediator.Request(new CreateServiceCallVerifyCustomerQuery
                {
                    HomeOwnerId = id
                });

            return View(model);
        }

        public ActionResult Create(Guid id)
        {
            var model = _mediator.Request(new CreateServiceCallQuery
                {
                    JobId = id
                });

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(CreateServiceCallModel model)
        {
            if (ModelState.IsValid)
            {
                var newCallId = _mediator.Send(new CreateServiceCallCommand{JobId = model.JobId, ServiceCallLineItems = model.ServiceCallLineItems.ToList().Select(x=>new ServiceCallLineItem{LineNumber = x.LineItemNumber, ProblemCode = x.ProblemCodeDisplayName, ProblemDescription = x.ProblemDescription})});
                if (_userSession.GetCurrentUser().IsInRole(UserRoles.WarrantyServiceManager) || _userSession.GetCurrentUser().IsInRole(UserRoles.WarrantyServiceCoordinator))
                { 
                    var notificationModel = _mediator.Request(new NewServiceCallAssignedToWsrNotificationQuery { ServiceCallId = newCallId });
                    _mailer.NewServiceCallAssignedToWsr(notificationModel).SendAsync();
                }

                return RedirectToAction("CallSummary", new {id = newCallId} );
            }

            return View(model);
        }

        public ActionResult Approve(Guid id)
        {
            _mediator.Send(new ServiceCallApproveCommand
            {
                ServiceCallId = id
            });

            var notificationModel = _mediator.Request(new NewServiceCallAssignedToWsrNotificationQuery { ServiceCallId = id });
            _mailer.NewServiceCallAssignedToWsr(notificationModel).SendAsync();

            return Json (new { success = "true"}, JsonRequestBehavior.AllowGet );
        }

        public ActionResult Deny(Guid id)
        {
            _mediator.Send(new ServiceCallDenyCommand
            {
                ServiceCallId = id
            });

            return Json(new { success = "true" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ToggleMarkAsSpecialProject(Guid id, string message)
        {
            _mediator.Send(new ServiceCallToogleSpecialProjectCommand
                {
                    ServiceCallId = id,
                    Text = message
                });
            return RedirectToAction("CallSummary", new {id});
        }
    }
}