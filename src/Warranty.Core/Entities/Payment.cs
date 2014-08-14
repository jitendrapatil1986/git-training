using System;

namespace Warranty.Core.Entities
{
    public class Payment : IAuditableEntity
    {
        public Guid PaymentId { get; set; }
        public string VendorNumber { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; }
        public string JobNumber { get; set; }
        public string JdeIdentifier { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}