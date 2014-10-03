namespace Warranty.Core.Features.JobSummary.Attachments
{
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using FileManagement;
    using NPoco;

    public class JobSummaryUploadAttachmentCommandHandler : ICommandHandler<JobSummaryUploadAttachmentCommand>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _logger;
        private readonly IFileSystemManager<Job> _jobFileManager;

        public JobSummaryUploadAttachmentCommandHandler(IDatabase database, IFileSystemManager<Job> jobFileManager, IActivityLogger logger)
        {
            _database = database;
            _jobFileManager = jobFileManager;
            _logger = logger;
        }

        public void Handle(JobSummaryUploadAttachmentCommand message)
        {
            using (_database)
            {
                var attachmentsFilePaths = _jobFileManager.MoveFilesToDirectoryAndRenameToAvoidCollisions(message.FileIds);
                foreach (var filePath in attachmentsFilePaths)
                {
                    var attachment = new JobAttachment
                        {
                            JobId = message.JobId,
                            FilePath = filePath,
                            DisplayName = System.IO.Path.GetFileName(filePath)
                        };

                    _database.Insert(attachment);
                    _logger.Write("Attachment added to Job", string.Format("File name: {0}", attachment.DisplayName), message.JobId, ActivityType.UploadAttachment, ReferenceType.Job);
                }
            }
        }
    }
}