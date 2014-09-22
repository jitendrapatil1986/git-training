namespace Warranty.Core.Entities
{
    using System;

    public class ServiceCallNote : IAuditableEntity
    {
        public virtual Guid ServiceCallNoteId { get; set; }
        public virtual Guid ServiceCallId { get; set; }
        public virtual string Note { get; set; }
        public virtual Guid? ServiceCallLineItemId { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
    }
}