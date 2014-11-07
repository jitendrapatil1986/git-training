using System;

namespace Warranty.Core.Entities
{
    using Enumerations;

    public class Payment : IAuditableEntity, IJdeEntity
    {
        public Guid PaymentId { get; set; }
        public string VendorNumber { get; set; }
        public string VendorName { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string JobNumber { get; set; }
        public string CommunityNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string HoldComments { get; set; }
        public Guid ServiceCallLineItemId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string JdeIdentifier { get; set; }
    }
}