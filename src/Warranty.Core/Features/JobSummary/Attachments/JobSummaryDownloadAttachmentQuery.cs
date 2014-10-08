namespace Warranty.Core.Features.JobSummary.Attachments
{
    using System;

    public class JobSummaryDownloadAttachmentQuery : IQuery<JobSummaryDownloadAttachmentModel>
    {
        public Guid JobId { get; set; }
    }
}