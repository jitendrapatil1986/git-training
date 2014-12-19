namespace Warranty.Core.Entities
{
    using System;

    public class VendorPhone : IAuditableEntity
    {
        public virtual Guid VendorPhoneId { get; set; }
        public virtual Guid VendorId { get; set; }
        public string Number { get; set; }
        public string Type { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
    }
}