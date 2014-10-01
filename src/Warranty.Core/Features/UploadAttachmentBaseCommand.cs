namespace Warranty.Core.Features
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Attributes;
    using FileManagement;

    public class UploadAttachmentBaseCommand
    {
        public UploadAttachmentBaseCommand()
        {
            TempFiles = new FileUpload();
        }

        public List<string> FileNames
        {
            get { return TempFiles.PendingFiles.Select(x => x.FileName).ToList(); }
        }

        public List<string> FileIds
        {
            get { return TempFiles.PendingFiles.Select(x => x.FileId).ToList(); }
        }

        [DisplayName("Files")]
        [CuteUiFileUploadOptions(AllowMultiple = false, FileTypes = "*.pdf,*.jpg,*.png,*.bmp,*.dwg,*.dwf,*.doc,*.xls,*.xlsx,*.docx")]
        public FileUpload TempFiles { get; set; }
    }
}