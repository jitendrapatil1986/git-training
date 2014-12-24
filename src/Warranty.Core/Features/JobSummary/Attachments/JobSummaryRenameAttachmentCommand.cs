namespace Warranty.Core.Features.JobSummary.Attachments
{
    using System;

    public class JobSummaryRenameAttachmentCommand : InlineEditCommandBase
    {
        public Guid JobId { get; set; }
    }
}