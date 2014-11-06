namespace Warranty.Core.ToDoInfrastructure.Models
{
    using System;
    using Enumerations;

    public class ToDoPaymentRequestApprovalModel
    {
        public Guid PaymentId { get; set; }
        public Guid ServiceCallLineItemId { get; set; }
        public Guid ServiceCallId { get; set; }
        public string ServiceCallNumber { get; set; }
        public string VendorName { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string HoldComments { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}