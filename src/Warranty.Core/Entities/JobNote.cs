namespace Warranty.Core.Entities
{
    using System;

    public class JobNote: IAuditableEntity
    {
        public virtual Guid JobNoteId { get; set; }
        public virtual Guid JobId { get; set; }
        public virtual string Note { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
    }
}