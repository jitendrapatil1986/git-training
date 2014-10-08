namespace Warranty.Core.Features.JobSummary.Attachments
{
    using System;

    public class JobSummaryUploadAttachmentCommand : UploadAttachmentBaseCommand, ICommand
    {
        public Guid JobId { get; set; }
    }
}