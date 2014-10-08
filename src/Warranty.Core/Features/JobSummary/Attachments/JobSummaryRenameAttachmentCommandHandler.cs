namespace Warranty.Core.Features.JobSummary.Attachments
{
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using NPoco;

    public class JobSummaryRenameAttachmentCommandHandler : ICommandHandler<JobSummaryRenameAttachmentCommand>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _logger;

        public JobSummaryRenameAttachmentCommandHandler(IDatabase database, IActivityLogger logger)
        {
            _database = database;
            _logger = logger;
        }
        
        public void Handle(JobSummaryRenameAttachmentCommand message)
        {
            using (_database)
            {
                var attachment = _database.SingleById<JobAttachment>(message.Pk);
                if (attachment != null)
                {
                    var oldName = attachment.DisplayName;
                    attachment.DisplayName = message.Value;
                    _database.Update(attachment);
                    _logger.Write("Attachment renamed on Job", string.Format("Previous File Name: {0}, New Name: {1}", oldName, attachment.DisplayName), message.Pk, ActivityType.RenamedAttachment, ReferenceType.Job);
                }
            }
        }
    }
}