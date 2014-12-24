namespace Warranty.Core.Features.ServiceCallSummary.Attachments
{
    using System;

    public class ServiceCallDownloadAttachmentQuery : IQuery<ServiceCallDownloadAttachmentModel>
    {
        public Guid Id { get; set; }
    }
}