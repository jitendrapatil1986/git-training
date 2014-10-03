namespace Warranty.Core.Entities
{
    using System;

    public class ServiceCallAttachment : IAuditableEntity
    {
        public virtual Guid ServiceCallAttachmentId { get; set; }
        public virtual Guid ServiceCallId { get; set; }
        public virtual Guid ServiceCallLineItemId { get; set; }
        public virtual string FilePath { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual bool IsDeleted { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
    }
}