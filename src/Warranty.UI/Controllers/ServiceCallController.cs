namespace Warranty.UI.Controllers
{
    using System;
    using System.Web.Mvc;
    using Warranty.Core;
    using Warranty.Core.Features.CreateServiceCall;
    using Warranty.Core.Features.CreateServiceCallCustomerSearch;
    using Warranty.Core.Features.CreateServiceCallVerifyCustomer;
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

        public ActionResult SearchCustomer(string searchCriteria)
        {
            var model = _mediator.Request(new CreateServiceCallCustomerSearchQuery
                {
                    SearchCriteria = searchCriteria
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
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

    }
}