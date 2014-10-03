namespace Warranty.Core.Features.ServiceCallSummary.Attachments
{
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using NPoco;

    public class ServiceCallRenameAttachmentCommandHandler : ICommandHandler<ServiceCallRenameAttachmentCommand>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _logger;

        public ServiceCallRenameAttachmentCommandHandler(IDatabase database, IActivityLogger logger)
        {
            _database = database;
            _logger = logger;
        }

        public void Handle(ServiceCallRenameAttachmentCommand message)
        {
            using (_database)
            {
                var attachment = _database.SingleById<ServiceCallAttachment>(message.Pk);
                if (attachment != null)
                {
                    var oldName = attachment.DisplayName;
                    attachment.DisplayName = message.Value;
                    _database.Update(attachment);
                    _logger.Write("Attachment renamed on Service Call", string.Format("Previous File Name: {0}, New name: {1}", oldName, attachment.DisplayName), message.Pk, ActivityType.RenamedAttachment, ReferenceType.ServiceCallAttachment);

                }
            }
        }
    }
}