namespace Warranty.Core.Features.JobSummary.Attachments
{
    using System;

    public class JobSummaryDeleteAttachmentCommand : ICommand
    {
        public Guid Id { get; set; }
    }
}