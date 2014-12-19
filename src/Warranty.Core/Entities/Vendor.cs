namespace Warranty.Core.Entities
{
    using System;

    public class Vendor : IAuditableEntity
    {
        public virtual Guid VendorId { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
    }
}