namespace Warranty.UI.Controllers
{
    using System;
    using System.Web.Mvc;
    using Warranty.Core;
    using Warranty.Core.Features.TaskCompletion;

    public class TaskController : Controller
    {
        private readonly IMediator _mediator;

        public TaskController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public ActionResult Complete(Guid id)
        {
            _mediator.Send(new TaskCompleteCommand
            {
                 TaskId = id
            });

            return Json (new { success = "true"}, JsonRequestBehavior.AllowGet );
        }
    }
}