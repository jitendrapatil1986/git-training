using Warranty.Core.Features.ServiceCallApproval;

namespace Warranty.UI.Controllers
{
    using System;
    using System.Web.Mvc;
    using Warranty.Core;
    using Warranty.Core.Features.ServiceCallSummary;

    public class ServiceCallController : Controller
    {
        private readonly IMediator _mediator;

        public ServiceCallController(IMediator mediator)
        {
            _mediator = mediator;
        }
         public ActionResult Reassign(Guid id)
         {
             return View();
         }

         public ActionResult AddNote(Guid id)
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


        public ActionResult Approve(Guid id)
        {
            _mediator.Send(new ServiceCallApproveCommand
            {
                ServiceCallId = id
            });

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
    }
}