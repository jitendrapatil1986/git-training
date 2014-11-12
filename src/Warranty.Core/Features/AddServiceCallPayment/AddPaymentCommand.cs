namespace Warranty.Core.Features.AddServiceCallPayment
{
    using System;

    public class AddPaymentCommand : ICommand<Guid>
    {
        public Guid ServiceCallLineItemId { get; set; }
        public string VendorNumber { get; set; }
        public string VendorName { get; set; }
        public string BackchargeVendorNumber { get; set; }
        public string BackchargeVendorName { get; set; }
        public string InvoiceNumber { get; set; }
        public bool IsBackcharge { get; set; }
        public decimal Amount { get; set; }
        public decimal BackchargeAmount { get; set; }
        public string BackchargeReason { get; set; }
        public string PersonNotified { get; set; }
        public string PersonNotifiedPhoneNumber { get; set; }
        public DateTime PersonNotifiedDate { get; set; }
        public string BackchargeResponseFromVendor { get; set; }
        public int SelectedCostCode { get; set; }
    }
}