namespace Warranty.Core.Entities
{
    using System;
    using Enumerations;

    public class HomeownerContact : IAuditableEntity
    {
        public virtual Guid HomeownerContactId { get; set; }
        public virtual Guid HomeownerId { get; set; }
        public virtual HomeownerContactType ContactType { get; set; }
        public virtual string ContactValue { get; set; }
        public virtual string ContactLabel { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
    }
}