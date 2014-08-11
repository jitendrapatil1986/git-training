namespace Warranty.Core.Entities
{
    using System;

    public class ServiceCallComment : IAuditableEntity
    {
        public virtual Guid ServiceCallCommentId { get; set; }
        public virtual Guid ServiceCallId { get; set; }
        public virtual string Comment { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
    }
}