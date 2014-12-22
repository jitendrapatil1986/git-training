using System;
using System.Collections.Generic;

namespace Warranty.Core.Features.ServiceCallSummary.ServiceCallLineItem
{
    using System.Linq;
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
        public string RootProblem { get; set; }
        public string ProblemDescription { get; set; }
        public string CauseDescription { get; set; }
        public string ClassificationNote { get; set; }
        public string LineItemRoot { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool CanReopenLines { get; set; }
        public bool CanTakeActionOnPayments { get; set; }
        public string CityCode { get; set; }
        public ServiceCallLineItemStatus ServiceCallLineItemStatus { get; set; }
        public IEnumerable<SelectListItem> ProblemCodes { get; set; }
        public IEnumerable<ServiceCallLineItemNote> ServiceCallLineItemNotes { get; set; }
        public IEnumerable<ServiceCallLineItemAttachment> ServiceCallLineItemAttachments { get; set; }
        public IEnumerable<ServiceCallLineItemPayment> ServiceCallLineItemPayments { get; set; }
        public IEnumerable<ServiceCallLineItemPurchaseOrder> ServiceCallLineItemPurchaseOrders { get; set; }
        public IEnumerable<Vendor> Vendors { get; set; }

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
            public BackchargeStatus BackchargeStatus { get; set; }
            public string PaymentStatusDisplayName { get { return PaymentStatus.DisplayName; }}
            public string BackchargeStatusDisplayName { get { return IsBackcharge ? BackchargeStatus.DisplayName : string.Empty; } }
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
            public string BackchargeHoldComments { get; set; }
            public DateTime? BackchargeHoldDate { get; set; }
            public string BackchargeDenyComments { get; set; }
            public DateTime? BackchargeDenyDate { get; set; }
            public bool IsBackcharge { get; set; }
        }

        public class ServiceCallLineItemPurchaseOrder
        {
            public Guid PurchaseOrderId { get; set; }
            public string PurchaseOrderNumber { get; set; }
            public string VendorNumber { get; set; }
            public string VendorName { get; set; }
            public DateTime? CreatedDate { get; set; }
            public List<ServiceCallLineItemPurchaseOrderLine> ServiceCallLineItemPurchaseOrderLines { get; set; }
            public decimal TotalCost { get { return ServiceCallLineItemPurchaseOrderLines.Sum(x => x.Quantity * x.UnitCost); } }
            public PurchaseOrderLineItemStatus  PurchaseOrderStatus {
                get
                {
                    return ServiceCallLineItemPurchaseOrderLines.All(x => x.PurchaseOrderLineItemStatus.Equals(PurchaseOrderLineItemStatus.Closed)) ? PurchaseOrderLineItemStatus.Closed :
                        PurchaseOrderLineItemStatus.Open;
                }
            }

            public string PurchaseOrderStatusDisplayName { get { return PurchaseOrderStatus.DisplayName; } }
        }

        public class ServiceCallLineItemPurchaseOrderLine
        {
            public Guid PurchaseOrderLineItemId { get; set; }
            public Guid PurchaseOrderId { get; set; }
            public int LineNumber { get; set; }
            public decimal Quantity { get; set; }
            public decimal UnitCost { get; set; }
            public PurchaseOrderLineItemStatus PurchaseOrderLineItemStatus { get; set; }
        }

        public class Vendor : IEquatable<Vendor>
        {
            public Guid VendorId { get; set; }
            public string Name { get; set; }
            public string Number { get; set; }
            public IList<ContactInfoModel> ContactInfo { get; set; }
            public IList<CostCodeModel> CostCodes { get; set; }

            public class ContactInfoModel : IEquatable<ContactInfoModel>
            {
                public string Value { get; set; }
                public string Type { get; set; }

                public bool Equals(ContactInfoModel other)
                {
                    return Value == other.Value && Type == other.Type;
                }

                public override int GetHashCode()
                {
                    return Value.GetHashCode() ^ Type.GetHashCode();
                }
            }

            public bool Equals(Vendor other)
            {
                return Number == other.Number;
            }

            public override int GetHashCode()
            {
                return Number.GetHashCode();
            }
        }
        public class CostCodeModel : IEquatable<CostCodeModel>
        {
            public string CostCode { get; set; }
            public string CostCodeDescription { get; set; }

            public bool Equals(CostCodeModel other)
            {
                return CostCode == other.CostCode;
            }

            public override int GetHashCode()
            {
                return CostCode.GetHashCode();
            }
        }
    }
}
