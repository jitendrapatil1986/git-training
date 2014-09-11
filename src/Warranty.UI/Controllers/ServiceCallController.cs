﻿using Warranty.Core.Features.ServiceCallApproval;

namespace Warranty.UI.Controllers
{
    using System;
    using System.Web.Mvc;
    using Warranty.Core;
    using Warranty.Core.Entities;
    using Warranty.Core.Features.AddServiceCallLineItem;
    using Warranty.Core.Features.CreateServiceCall;
    using Warranty.Core.Features.CreateServiceCallCustomerSearch;
    using Warranty.Core.Features.CreateServiceCallVerifyCustomer;
    using Warranty.Core.Features.ServiceCallSummary;
    using System.Linq;

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
                return RedirectToAction("CallSummary", new {id = newCallId} );
            }

            return View();
        }

        [HttpPost]
        public JsonResult AddLineItem(AddServiceCallLineItemModel model)
        {
            var result = _mediator.Send(new AddServiceCallLineItemCommand
                {
                    ServiceCallId = model.ServiceCallId, ProblemCode = model.ProblemCode, ProblemDescription = model.ProblemDescription
                });

            return Json(new {newServiceLineId = result}, JsonRequestBehavior.AllowGet);
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