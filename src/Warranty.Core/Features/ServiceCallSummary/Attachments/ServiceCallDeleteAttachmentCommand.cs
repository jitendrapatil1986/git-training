namespace Warranty.Core.Features.ServiceCallSummary.Attachments
{
    using System;

    public class ServiceCallDeleteAttachmentCommand : ICommand
    {
        public Guid Id { get; set; }
    }
}