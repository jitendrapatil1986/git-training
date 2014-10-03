namespace Warranty.UI
{
    using Core.Helpers;
    using CuteWebUI;

    public class UploadHandler : MvcHandler
    {
        public override UploaderValidateOption GetValidateOption()
        {
            return new UploaderValidateOption();
        }

        public override void OnUploaderInit(MvcUploader uploader) { }

        public override void OnFileUploaded(MvcUploadFile file)
        {
            UploadHelper.MoveFileToUploadDirectory(file);
        }
    }
}