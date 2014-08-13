using System;
using NPoco;

namespace Warranty.Core.Entities
{
    [TableName("Payments")]
    [PrimaryKey("PaymentId")]
    public class Payment : IAuditableEntity
    {
        public virtual Guid PaymentId { get; set; }
        public virtual string VendorNumber { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual string PaymentStatus { get; set; }
        public virtual string JobNumber { get; set; }
        public virtual string JdeIdentifier { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
    }
}