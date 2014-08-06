using System;

namespace Warranty.Core.Entities
{
    public class WarrantyCallComment : IAuditableEntity
    {
        public virtual Guid WarrantyCallCommentId { get; set; }
        public virtual Guid WarrantyCallId { get; set; }
        public virtual string Comment { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
    }
}