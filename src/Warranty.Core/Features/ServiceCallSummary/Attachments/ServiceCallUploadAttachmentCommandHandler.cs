namespace Warranty.Core.Features.ServiceCallSummary.Attachments
{
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using FileManagement;
    using NPoco;

    public class ServiceCallUploadAttachmentCommandHandler : ICommandHandler<ServiceCallUploadAttachmentCommand>
    {
        private readonly IDatabase _database;
        private readonly IFileSystemManager<ServiceCall> _serviceCallFileManager;
        private readonly IActivityLogger _logger;

        public ServiceCallUploadAttachmentCommandHandler(IDatabase database, IFileSystemManager<ServiceCall> serviceCallFileManager, IActivityLogger logger)
        {
            _database = database;
            _serviceCallFileManager = serviceCallFileManager;
            _logger = logger;
        }

        public void Handle(ServiceCallUploadAttachmentCommand message)
        {
            using (_database)
            {
                var attachmentsFilePaths = _serviceCallFileManager.MoveFilesToDirectoryAndRenameToAvoidCollisions(message.FileIds);
                foreach (var filePath in attachmentsFilePaths)
                {
                    var attachment = new ServiceCallAttachment
                        {
                            ServiceCallId = message.ServiceCallId,
                            FilePath = filePath,
                            DisplayName = System.IO.Path.GetFileName(filePath),
                            ServiceCallLineItemId = message.ServiceCallLineItemId
                        };
                    _database.Insert(attachment);
                    _logger.Write("Attachment added to Service Call",string.Format("File name: {0}",attachment.DisplayName),message.ServiceCallId, ActivityType.UploadAttachment, ReferenceType.ServiceCallAttachment);
                }
            }
        }
    }
}