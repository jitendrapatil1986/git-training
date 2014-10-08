namespace Warranty.Core.Features.JobSummary.Attachments
{
    using System.Web;
    using Entities;
    using NPoco;

    public class JobSummaryDownloadAttachmentQueryHandler : IQueryHandler<JobSummaryDownloadAttachmentQuery, JobSummaryDownloadAttachmentModel>
    {
        private readonly IDatabase _database;

        public JobSummaryDownloadAttachmentQueryHandler(IDatabase database)
        {
            _database = database;
        }
        
        public JobSummaryDownloadAttachmentModel Handle(JobSummaryDownloadAttachmentQuery query)
        {
            using (_database)
            {
                var attachment = _database.SingleById<JobAttachment>(query.JobId);
                return new JobSummaryDownloadAttachmentModel
                    {
                        Bytes = System.IO.File.ReadAllBytes(attachment.FilePath),
                        FileName = attachment.DisplayName,
                        MimeMapping = MimeMapping.GetMimeMapping(attachment.FilePath)
                    };
            }
        }
    }
}