using System;
using System.Collections.Generic;

namespace Warranty.Core.Features.ServiceCallSummary.ServiceCallLineItem
{
    using System.Web.Mvc;
    using Enumerations;

    public class ServiceCallLineItemModel : UploadAttachmentBaseViewModel
    {
        public Guid ServiceCallLineItemId { get; set; }
        public Guid ServiceCallId { get; set; }
        public string ServiceCallNumber { get; set; }
        public int LineNumber { get; set; }
        public string ProblemCode { get; set; }
        public string ProblemJdeCode { get; set; }
        public string ProblemDetailCode { get; set; }
        public string CostCode { get; set; }
        public string JobNumber { get; set; }
        public string RootCause { get; set; }
        public string ProblemDescription { get; set; }
        public string CauseDescription { get; set; }
        public string ClassificationNote { get; set; }
        public string LineItemRoot { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool CanReopenLines { get; set; }
        public ServiceCallLineItemStatus ServiceCallLineItemStatus { get; set; }
        public IEnumerable<SelectListItem> ProblemCodes { get; set; }
        public IEnumerable<ServiceCallLineItemNote> ServiceCallLineItemNotes { get; set; }
        public IEnumerable<ServiceCallLineItemAttachment> ServiceCallLineItemAttachments { get; set; }
        public IEnumerable<ServiceCallLineItemPayment> ServiceCallLineItemPayments { get; set; }

        public class ServiceCallLineItemNote
        {
            public Guid ServiceCallNoteId { get; set; }
            public Guid ServiceCallId { get; set; }
            public string Note { get; set; }
            public Guid ServiceCallLineItemId { get; set; }
            public DateTime? CreatedDate { get; set; }
            public string CreatedBy { get; set; }
        }

        public class ServiceCallLineItemAttachment
        {
            public Guid ServiceCallId { get; set; }
            public Guid ServiceCallAttachmentId { get; set; }
            public Guid ServiceCallLineItemId { get; set; }
            public string DisplayName { get; set; }
            public DateTime? CreatedDate { get; set; }
            public string CreatedBy { get; set; }
        }

        public class ServiceCallLineItemPayment
        {
            public Guid PaymentId { get; set; }
            public string VendorNumber { get; set; }
            public decimal Amount { get; set; }
            public PaymentStatus PaymentStatus { get; set; }
            public string PaymentStatusDisplayName { get { return PaymentStatus.DisplayName; }}
            public string InvoiceNumber { get; set; }
            public Guid ServiceCallLineItemId { get; set; }
            public DateTime PaymentCreatedDate { get; set; }
            public Guid BackchargeId { get; set; }
            public string BackchargeVendorNumber { get; set; }
            public decimal BackchargeAmount { get; set; }
            public string BackchargeReason { get; set; }
            public string PersonNotified { get; set; }
            public string PersonNotifiedPhoneNumber { get; set; }
            public DateTime PersonNotifiedDate { get; set; }
            public string BackchargeResponseFromVendor { get; set; }
            public string VendorName { get; set; }
            public string BackchargeVendorName { get; set; }
            public string HoldComments { get; set; }
            public DateTime? HoldDate { get; set; }
            public bool IsBackcharge { get; set; }
        }

    }
}
