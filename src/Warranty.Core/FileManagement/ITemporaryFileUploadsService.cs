namespace Warranty.Core.FileManagement
{
    using System.IO;

    public interface ITemporaryFileUploadsService
    {
        string GetOriginalFileName(string fileId);
        FileInfo GetTemporaryFile(string fileId);
        FileInfo MoveToOriginalFileName(string fileId);
    }
}