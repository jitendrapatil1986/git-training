namespace Warranty.Core.ToDoInfrastructure.Models
{
    using System;
    using Enumerations;

    public class ToDoPaymentStatusChangedModel
    {
        public PaymentStatus PaymentStatus { get; set; }
        public decimal Amount { get; set; }
        public Guid TaskId { get; set; }
        public string Description { get; set; }
        public string HomeOwnerName { get; set; }
        public int HomeOwnerNumber { get; set; }
        public string AddressLine { get; set; }
        public Guid? JobId { get; set; }
        public int JobNumber { get; set; }
        public Guid ServiceCallLineItemId { get; set; }
        public string CssClassName { get { return PaymentStatus == PaymentStatus.Approved ? "success" : "warning"; } }
    }
}