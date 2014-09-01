using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Warranty.UI.Controllers
{
    using Warranty.Core;
    using Warranty.Core.Features.JobSummary;

    public class JobController : Controller
    {
        private readonly IMediator _mediator;

        public JobController(IMediator mediator)
        {
            _mediator = mediator;
        }
       
        public ActionResult JobSummary(Guid id)
        {
            var model = _mediator.Request(new JobSummaryQuery
                {
                    JobId = id
                });

            return View(model);
        }
    }
}
