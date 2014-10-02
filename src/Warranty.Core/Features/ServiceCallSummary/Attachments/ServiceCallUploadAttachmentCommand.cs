namespace Warranty.Core.Features.ServiceCallSummary.Attachments
{
    using System;

    public class ServiceCallUploadAttachmentCommand : UploadAttachmentBaseCommand, ICommand
    {
        public Guid ServiceCallId { get; set; }
        public Guid ServiceCallLineItemId { get; set; }
    }
}