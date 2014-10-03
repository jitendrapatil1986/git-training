namespace Warranty.Core.Features.JobSummary.Attachments
{
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using NPoco;

    public class JobSummaryDeleteAttachmentCommandHandler : ICommandHandler<JobSummaryDeleteAttachmentCommand>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _logger;

        public JobSummaryDeleteAttachmentCommandHandler(IDatabase database, IActivityLogger logger)
        {
            _database = database;
            _logger = logger;
        }

        public void Handle(JobSummaryDeleteAttachmentCommand message)
        {
            using (_database)
            {
                var attachment = _database.SingleById<JobAttachment>(message.Id);

                if (attachment != null)
                {
                    attachment.IsDeleted = true;
                    _database.Update(attachment);
                    _logger.Write("Attachment deleted from Job", string.Format("File name: {0}", attachment.DisplayName), message.Id, ActivityType.DeletedAttachment, ReferenceType.Job);
                }
            }
        }
    }
}