namespace Warranty.Core.Features.JobSummary.Attachments
{
    public class JobSummaryDownloadAttachmentModel
    {
        public byte[] Bytes { get; set; }
        public string MimeMapping { get; set; }
        public string FileName { get; set; }
    }
}