namespace Warranty.Core.FileManagement
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Web;

    public class TemporaryFileUploadsService : ITemporaryFileUploadsService
    {
        public FileInfo MoveToOriginalFileName(string fileId)
        {
            var uniqueFileInfo = GetTemporaryFile(fileId);

            var newSubDirectory = uniqueFileInfo.Directory.CreateSubdirectory(Guid.NewGuid().ToString("N"));

            var newFullFileName = Path.Combine(newSubDirectory.FullName, GetOriginalFileName(fileId));

            uniqueFileInfo.MoveTo(newFullFileName);

            return new FileInfo(newFullFileName);
        }

        public string GetOriginalFileName(string fileId)
        {
            var fileInfo = GetTemporaryFile(fileId);

            return fileInfo.Name.Substring(32, fileInfo.Name.Length - 32);
        }

        public FileInfo GetTemporaryFile(string fileId)
        {
            var uploadDirectory = GetTempUploadDirectory();

            var fileIdForFile = fileId.Replace("-", "");

            var fileInfo = uploadDirectory.EnumerateFiles().FirstOrDefault(x => x.Name.StartsWith(fileIdForFile));

            if (fileInfo == null)
            {
                // Cute Web UI seems to have a race conditon in a web farm environment, 
                // when using the iframe mode of renderer, where the last file doesn't arrive 
                // at the server but it posts the form indicating the file is on the disk.
                // for these rare cases, we want to just sleep and try again. We suspect it only needs
                // a few milliseconds to get the file, but are increasing it to 5 figuring it will 
                // definitely be there by then

                Thread.Sleep(5 * 1000);

                fileInfo = uploadDirectory.EnumerateFiles().FirstOrDefault(x => x.Name.StartsWith(fileIdForFile));

                if (fileInfo == null)
                {
                    throw new Exception(string.Format("Could not find temporary file with id '{0}' in location '{1}'.", fileId, uploadDirectory));
                }
            }

            return fileInfo;
        }

        private static DirectoryInfo GetTempUploadDirectory()
        {
            var path = ConfigurationManager.AppSettings["CuteWebUI.AjaxUploader.TempDirectory"];

            if (path.StartsWith("~") || path.StartsWith("/"))
                path = HttpContext.Current.Server.MapPath(path);

            var directory = new DirectoryInfo(path);

            if (!directory.Exists)
                directory.Create();

            return directory;
        }
    }
}