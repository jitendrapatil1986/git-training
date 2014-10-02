namespace Warranty.Core.Features.ServiceCallSummary.Attachments
{
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using FileManagement;
    using NPoco;

    public class ServiceCallUploadAttachmentCommandHandler : ICommandHandler<ServiceCallUploadAttachmentCommand>
    {
        private readonly IDatabase _datatabse;
        private readonly IFileSystemManager<ServiceCall> _serviceCallFileManager;
        private readonly IActivityLogger _logger;

        public ServiceCallUploadAttachmentCommandHandler(IDatabase datatabse, IFileSystemManager<ServiceCall> serviceCallFileManager, IActivityLogger logger)
        {
            _datatabse = datatabse;
            _serviceCallFileManager = serviceCallFileManager;
            _logger = logger;
        }

        public void Handle(ServiceCallUploadAttachmentCommand message)
        {
            using (_datatabse)
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
                    _datatabse.Insert(attachment);
                    _logger.Write("Attachment added to Service Call",string.Format("File name: {0}",attachment.DisplayName),message.ServiceCallId, ActivityType.UploadAttachment, ReferenceType.ServiceCallAttachment);
                }
            }
        }
    }
}