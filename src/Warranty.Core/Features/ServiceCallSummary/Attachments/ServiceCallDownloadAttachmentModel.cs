namespace Warranty.Core.Features.ServiceCallSummary.Attachments
{
    public class ServiceCallDownloadAttachmentModel
    {
        public byte[] Bytes { get; set; }
        public string MimeMapping { get; set; }
        public string FileName { get; set; }
    }
}