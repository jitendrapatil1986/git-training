namespace Warranty.UI.Controllers
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using Mailers;
    using Warranty.Core;
    using Warranty.Core.Entities;
    using Warranty.Core.Enumerations;
    using Warranty.Core.Features.CreateServiceCall;
    using Warranty.Core.Features.CreateServiceCallCustomerSearch;
    using Warranty.Core.Features.CreateServiceCallVerifyCustomer;
    using Warranty.Core.Features.ServiceCallSummary;
    using System.Linq;
    using Warranty.Core.Features.ServiceCallSummary.Attachments;
    using Warranty.Core.Security;
    using Warranty.Core.Features.ServiceCallApproval;
    using Warranty.Core.Features.ServiceCallSummary.ReassignEmployee;
    using Warranty.Core.Features.ServiceCallToggleActions;

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
                    if (notificationModel.WarrantyRepresentativeEmployeeEmail != null)
                    {
                        _mailer.NewServiceCallAssignedToWsr(notificationModel).SendAsync();
                    }
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

        public ActionResult ToggleSpecialProject(Guid id, string message)
        {
            _mediator.Send(new ServiceCallToggleSpecialProjectCommand
            {
                ServiceCallId = id,
                Text = message
            });
            return Json(new { actionName = ActivityType.SpecialProject.DisplayName, actionMessage = message }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ToggleEscalate(Guid id, string message)
        {
            var result = _mediator.Send(new ServiceCallToggleEscalateCommand
            {
                ServiceCallId = id,
                Text = message
            });

            if (result.ShouldSendEmail)
            {
                var urlHelper = new UrlHelper(this.ControllerContext.RequestContext);
                var url = urlHelper.Action("CallSummary", "ServiceCall", new { id }, Request.Url.Scheme);
                result.Url = url;
                _mailer.ServiceCallEscalated(result).SendAsync();
            }
            return Json(new { actionName = ActivityType.Escalation.DisplayName, actionMessage = message }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEmployees(Guid id)
        {
            var employees = _mediator.Request(new EmployeesForServiceCallQuery
            {
                ServiceCallId = id
            });

            return Json(employees.Select(x => new { value = x.EmployeeNumber, text = x.DisplayName }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult InlineReassign(ReassignEmployeeCommand command)
        {
            _mediator.Send(command);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Complete(Guid id)
        {
            _mediator.Send(new ServiceCallCompleteCommand()
            {
                ServiceCallId = id,
            });
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Reopen(Guid id, string message)
        {
            _mediator.Send(new ServiceCallReopenCommand
            {
                ServiceCallId = id,
                Text = message

            });
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadAttachment(ServiceCallUploadAttachmentCommand model)
        {
            _mediator.Send(model);
            return RedirectToAction("CallSummary", new {id = model.ServiceCallId});
        }

        [HttpPost]
        public ActionResult RenameAttachment(ServiceCallRenameAttachmentCommand model)
        {
            _mediator.Send(model);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteAttachment(ServiceCallDeleteAttachmentCommand model)
        {
            _mediator.Send(model);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadAttachment(Guid id)
        {
            var model = _mediator.Request(new ServiceCallDownloadAttachmentQuery { Id = id });
            return File(model.Bytes,model.MimeMapping, model.FileName);
        }
    }
}