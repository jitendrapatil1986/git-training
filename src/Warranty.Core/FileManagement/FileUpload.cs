namespace Warranty.Core.FileManagement
{
    using System.Collections.Generic;
    using System.Linq;

    public class FileUpload
    {
        public string RawPendingUploads { get; set; }

        public IEnumerable<TemporaryFileUploadInfo> PendingFiles { get; set; }
        public List<string> PendingFileIds { get; set; }
        public string[] PreviousFiles { get; set; }

        public FileUpload()
        {
            PendingFiles = new List<TemporaryFileUploadInfo>();
            PreviousFiles = Enumerable.Empty<string>().ToArray();
            PendingFileIds = new List<string>();
        }

        public bool HasPreviousOrPendingFiles
        {
            get { return PendingFiles.Any() || PreviousFiles.Any(); }
        }
    }
}