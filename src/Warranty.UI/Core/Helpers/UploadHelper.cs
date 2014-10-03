namespace Warranty.UI.Core.Helpers
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Web;
    using CuteWebUI;

    public static class UploadHelper
    {
        public static void MoveFileToUploadDirectory(MvcUploadFile file)
        {
            var uploadDirectory = GetUploadDirectory();
            var fileName = file.FileGuid.ToString("N") + file.FileName;
            var filePath = Path.Combine(uploadDirectory.FullName, fileName);
            file.MoveTo(filePath);
        }

        public static DirectoryInfo GetUploadDirectory()
        {
            return GetDirectoryFromConfigFile("CuteWebUI.AjaxUploader.TempDirectory");
        }

        public static DirectoryInfo GetDirectoryFromConfigFile(string configSetting)
        {
            var path = ConfigurationManager.AppSettings[configSetting];

            if (path.StartsWith("~") || path.StartsWith("/"))
                path = HttpContext.Current.Server.MapPath(path);

            var directory = new DirectoryInfo(path);

            if (!directory.Exists)
                directory.Create();

            return directory;
        }

        public static FileInfo GetUploadedFile(Guid fileIdentifier)
        {
            var uploadDirectory = GetUploadDirectory();
            return uploadDirectory.EnumerateFiles().SingleOrDefault(x => x.Name.StartsWith(fileIdentifier.ToString("N")));
        }

        public static void DeleteUploadedFile(Guid fileIdentifier)
        {
            if (fileIdentifier == Guid.Empty)
                return;

            var file = GetUploadedFile(fileIdentifier);
            if (file != null)
                file.Delete();
        }
    }
}
