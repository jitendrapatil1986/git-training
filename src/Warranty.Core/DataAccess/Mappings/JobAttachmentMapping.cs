namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;
    using NHibernate.Mapping.ByCode;

    public class JobAttachmentMapping: AuditableEntityMapping<JobAttachment>
    {
        public JobAttachmentMapping()
        {
            Table("JobAttachments");

            Id(x => x.JobAttachmentId, map => map.Generator(Generators.GuidComb));
            Property(x => x.JobId);
            Property(x => x.FilePath);
            Property(x => x.DisplayName);
            Property(x => x.IsDeleted);
        }
    }
}