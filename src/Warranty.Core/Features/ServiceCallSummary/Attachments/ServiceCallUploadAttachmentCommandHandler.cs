namespace Warranty.Core.Features.ServiceCallSummary.Attachments
{
    using Entities;
    using FileManagement;
    using NPoco;

    public class ServiceCallUploadAttachmentCommandHandler : ICommandHandler<ServiceCallUploadAttachmentCommand>
    {
        private readonly IDatabase _datatabse;
        private readonly IFileSystemManager<ServiceCall> _serviceCallFileManager;

        public ServiceCallUploadAttachmentCommandHandler(IDatabase datatabse, IFileSystemManager<ServiceCall> serviceCallFileManager)
        {
            _datatabse = datatabse;
            _serviceCallFileManager = serviceCallFileManager;
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
                }
            }
        }
    }
}