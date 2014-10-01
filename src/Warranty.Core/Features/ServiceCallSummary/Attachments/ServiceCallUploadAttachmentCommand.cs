namespace Warranty.Core.Features.ServiceCallSummary.Attachments
{
    using Entities;
    using FileManagement;

    public class ServiceCallUploadAttachmentCommand : UploadAttachmentBaseCommand, ICommand
    {
    }

    public class ServiceCallUploadAttachmentCommandHandler : ICommandHandler<ServiceCallUploadAttachmentCommand>
    {
        private readonly IFileSystemManager<ServiceCall> _serviceCallFileManager;

        public ServiceCallUploadAttachmentCommandHandler(IFileSystemManager<ServiceCall> serviceCallFileManager )
        {
            _serviceCallFileManager = serviceCallFileManager;
        }

        public void Handle(ServiceCallUploadAttachmentCommand message)
        {
            var correctFileNames = _serviceCallFileManager.MoveFilesToDirectoryAndRenameToAvoidCollisions(message.FileIds);
        }
    }
}