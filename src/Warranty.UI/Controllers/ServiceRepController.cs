using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Warranty.Core;
using Warranty.Core.Features.RepServiceCalls;

namespace Warranty.UI.Controllers
{
    public class ServiceRepController : Controller
    {
        private readonly IMediator _mediator;

        public ServiceRepController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public ActionResult ServiceCalls(Guid id)
        {
            var model = _mediator.Request(new ServiceRepServiceCallsQuery
                {
                    EmployeeId = id
                });
            return View(model);
        }

    }
}
