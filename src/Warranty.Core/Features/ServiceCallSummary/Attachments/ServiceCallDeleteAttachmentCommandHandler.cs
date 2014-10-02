namespace Warranty.Core.Features.ServiceCallSummary.Attachments
{
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using NPoco;

    public class ServiceCallDeleteAttachmentCommandHandler : ICommandHandler<ServiceCallDeleteAttachmentCommand>
    {
        private readonly IDatabase _datatabse;
        private readonly IActivityLogger _logger;

        public ServiceCallDeleteAttachmentCommandHandler(IDatabase datatabse, IActivityLogger logger)
        {
            _datatabse = datatabse;
            _logger = logger;
        }

        public void Handle(ServiceCallDeleteAttachmentCommand message)
        {
            using (_datatabse)
            {
                var attachment = _datatabse.SingleById<ServiceCallAttachment>(message.Id);
                if (attachment != null)
                {
                    attachment.IsDeleted = true;
                    _datatabse.Update(attachment);
                    _logger.Write("Attachment deleted from Service Call", string.Format("File name: {0}", attachment.DisplayName), message.Id, ActivityType.DeletedAttachment, ReferenceType.ServiceCallAttachment);
                }
            }
        }
    }
}