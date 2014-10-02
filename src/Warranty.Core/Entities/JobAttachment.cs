namespace Warranty.Core.Entities
{
    using System;

    public class JobAttachment : IAuditableEntity
    {
        public virtual Guid JobAttachmentId { get; set; }
        public virtual Guid JobId { get; set; }
        public virtual string FilePath { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual bool IsDeleted { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
    }
}