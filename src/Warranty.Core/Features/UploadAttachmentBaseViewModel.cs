namespace Warranty.Core.Features
{
    using System.ComponentModel;
    using Attributes;
    using FileManagement;

    public class UploadAttachmentBaseViewModel
    {
        public UploadAttachmentBaseViewModel()
        {
            TempFiles = new FileUpload();
        }

        [DisplayName("Files")]
        [CuteUiFileUploadOptions(AllowMultiple = true, FileTypes = "*.pdf,*.jpeg,*.jpg,*.png,*.bmp,*.dwg,*.dwf,*.doc,*.xls,*.xlsx,*.docx")]
        public FileUpload TempFiles { get; set; }
    }
}