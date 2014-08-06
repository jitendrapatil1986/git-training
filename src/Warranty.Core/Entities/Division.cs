using System;

namespace Warranty.Core.Entities
{
    public class Division : IAuditableEntity
    {
        public virtual Guid DivisionId { get; set; }
        public virtual string DivisionCode { get; set; }
        public virtual string DivisionName { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
    }
}