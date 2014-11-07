namespace Warranty.Core.Entities
{
    using System;
    using Enumerations;

    public class Backcharge : IAuditableEntity
    {
        public Guid BackchargeId { get; set; }
        public Guid PaymentId { get; set; }
        public string BackchargeVendorNumber { get; set; }
        public string BackchargeVendorName { get; set; }
        public decimal BackchargeAmount { get; set; }
        public string BackchargeReason { get; set; }
        public string PersonNotified { get; set; }
        public string PersonNotifiedPhoneNumber { get; set; }
        public DateTime PersonNotifiedDate { get; set; }
        public string BackchargeResponseFromVendor { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string JdeIdentifier { get; set; }
        public string HoldComments { get; set; }
        public string DenyComments { get; set; }
        public BackchargeStatus BackchargeStatus { get; set; }
    }
}