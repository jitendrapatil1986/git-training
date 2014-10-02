namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

    public class JobAttachmentMap: AuditableEntityMap<JobAttachment>
    {
        public JobAttachmentMap()
        {
            TableName("JobAttachments")
                .PrimaryKey("JobAttachmentId")
                .Columns(x =>
                {
                    x.Column(y => y.JobId);
                    x.Column(y => y.FilePath);
                    x.Column(y => y.DisplayName);
                    x.Column(y => y.IsDeleted);
                });
        }
    }
}