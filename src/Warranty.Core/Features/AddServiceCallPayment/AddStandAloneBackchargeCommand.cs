namespace Warranty.Core.Features.AddServiceCallPayment
{
    using System;

    public class AddStandAloneBackchargeCommand : ICommand<AddStandAloneBackchargeCommandDto>
    {
        public Guid ServiceCallLineItemId { get; set; }
        public string BackchargeVendorNumber { get; set; }
        public string BackchargeVendorName { get; set; }
        public decimal BackchargeAmount { get; set; }
        public string BackchargeReason { get; set; }
        public string PersonNotified { get; set; }
        public string PersonNotifiedPhoneNumber { get; set; }
        public DateTime PersonNotifiedDate { get; set; }
        public string BackchargeResponseFromVendor { get; set; }
    }
}