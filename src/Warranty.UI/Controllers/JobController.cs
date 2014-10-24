using System;
using System.Web.Mvc;

namespace Warranty.UI.Controllers
{
    using Warranty.Core;
    using Warranty.Core.Features.JobSummary;
    using Warranty.Core.Features.JobSummary.Attachments;
    using Warranty.Core.Features.JobSummary.ChangeHomeowner;

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

        [HttpPost]
        public ActionResult UploadAttachment(JobSummaryUploadAttachmentCommand model)
        {
            _mediator.Send(model);
            return RedirectToAction("JobSummary", new {id = model.JobId});
        }

        [HttpPost]
        public ActionResult RenameAttachment(JobSummaryRenameAttachmentCommand model)
        {
            _mediator.Send(model);
            return Json(new {success = true}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteAttachment(JobSummaryDeleteAttachmentCommand model)
        {
            _mediator.Send(model);
            return Json(new {success = true}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadAttachment(Guid id)
        {
            var model = _mediator.Request(new JobSummaryDownloadAttachmentQuery {JobId = id});
            return File(model.Bytes, model.MimeMapping, model.FileName);
        }

        [HttpGet]
        public ActionResult ChangeHomeowner(Guid id)
        {
            var model = _mediator.Request(new ChangeHomeownerQuery
                {
                    JobId = id,
                });
            
            return View(model);
        }
    }
}
