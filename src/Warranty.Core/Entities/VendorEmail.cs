namespace Warranty.Core.Entities
{
    using System;

    public class VendorEmail : IAuditableEntity
    {
        public virtual Guid VendorEmailId { get; set; }
        public virtual Guid VendorId { get; set; }
        public string Email { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
    }
}