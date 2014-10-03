namespace Warranty.Core.Features.ServiceCallSummary.Attachments
{
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using NPoco;

    public class ServiceCallDeleteAttachmentCommandHandler : ICommandHandler<ServiceCallDeleteAttachmentCommand>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _logger;

        public ServiceCallDeleteAttachmentCommandHandler(IDatabase database, IActivityLogger logger)
        {
            _database = database;
            _logger = logger;
        }

        public void Handle(ServiceCallDeleteAttachmentCommand message)
        {
            using (_database)
            {
                var attachment = _database.SingleById<ServiceCallAttachment>(message.Id);
                if (attachment != null)
                {
                    attachment.IsDeleted = true;
                    _database.Update(attachment);
                    _logger.Write("Attachment deleted from Service Call", string.Format("File name: {0}", attachment.DisplayName), message.Id, ActivityType.DeletedAttachment, ReferenceType.ServiceCallAttachment);
                }
            }
        }
    }
}