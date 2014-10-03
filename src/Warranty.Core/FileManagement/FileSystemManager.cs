namespace Warranty.Core.FileManagement
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;

    public class FileSystemManager<TEntity> : IFileSystemManager<TEntity>
    {
        private readonly ITemporaryFileUploadsService _temporaryFileUploadsService;
        private readonly string _fileAttachmentsRootPath;

        public string EntityName { get; set; }

        public FileSystemManager(ITemporaryFileUploadsService temporaryFileUploadsService)
        {
            _temporaryFileUploadsService = temporaryFileUploadsService;
            _fileAttachmentsRootPath = ConfigurationManager.AppSettings["DocumentSharePath"];
            EntityName = typeof(TEntity).Name;
        }

        public string GetCollisionAndLengthSafeFileName(string path, string fileName)
        {
            const int maxLengthOfFile = 40;

            if (fileName.Length > maxLengthOfFile)
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                var fileNameExtension = Path.GetExtension(fileName);

                var truncatedFileNameLength = maxLengthOfFile - fileNameExtension.Length;
                var truncatedFileName = fileNameWithoutExtension.Substring(0, truncatedFileNameLength) + fileNameExtension;
                fileName = truncatedFileName;
            }

            var filePath = Path.Combine(path, fileName);
            var file = new FileInfo(filePath);

            var counter = 1;
            while (file.Exists)
            {
                var newFileName = Path.GetFileNameWithoutExtension(fileName);
                newFileName += "_" + (counter++) + Path.GetExtension(fileName);

                filePath = Path.Combine(path, newFileName);
                file = new FileInfo(filePath);
            }

            return file.Name;
        }

        public List<string> MoveFilesToDirectoryAndRenameToAvoidCollisions(TEntity entity, IEnumerable<string> temporaryFileIds, string newFriendlyName = null)
        {
            var destinationFilePath = Path.Combine(_fileAttachmentsRootPath, EntityName);
            return MoveFilesToDirectoryAndRenameToAvoidCollisions(destinationFilePath, temporaryFileIds, newFriendlyName);
        }

        public List<string> MoveFilesToDirectoryAndRenameToAvoidCollisions(IEnumerable<string> temporaryFileIds, string newFriendlyName = null)
        {
            var destinationFilePath = Path.Combine(_fileAttachmentsRootPath, EntityName);
            return MoveFilesToDirectoryAndRenameToAvoidCollisions(destinationFilePath, temporaryFileIds, newFriendlyName);
        }

        private List<string> MoveFilesToDirectoryAndRenameToAvoidCollisions(string destinationFilePath, IEnumerable<string> temporaryFileIds, string newFriendlyName = null)
        {
            if (!Directory.Exists(destinationFilePath))
                Directory.CreateDirectory(destinationFilePath);

            var correctedFileNames = new List<string>();
            foreach (var fileIds in temporaryFileIds)
            {
                var originalFileName = _temporaryFileUploadsService.GetOriginalFileName(fileIds);
                var newFileName = originalFileName;

                if (!string.IsNullOrWhiteSpace(newFriendlyName))
                {
                    newFileName = string.Format("{0}{1}", newFriendlyName, Path.GetExtension(originalFileName));
                }

                var collisionSafeScholarFileName = GetCollisionAndLengthSafeFileName(destinationFilePath, newFileName);
                correctedFileNames.Add(Path.Combine(destinationFilePath, collisionSafeScholarFileName));

                var temporaryFile = _temporaryFileUploadsService.GetTemporaryFile(fileIds);
                var fullyQualifiedFilePathAndName = Path.Combine(destinationFilePath, collisionSafeScholarFileName);
                temporaryFile.MoveTo(fullyQualifiedFilePathAndName);
            }

            return correctedFileNames;
        }

        public void DeleteAttachment(TEntity entity, string fileName)
        {
            var filePath = Path.Combine(_fileAttachmentsRootPath, EntityName, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}