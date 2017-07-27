using System;
using System.Collections.Generic;

namespace Warranty.Core.Features.JobSummary
{
    using Enumerations;
    using Services;

    public class JobSummaryModel : UploadAttachmentBaseViewModel
    {
        public Guid JobId { get; set; }
        public string JobNumber { get; set; }
        public int? Stage { get; set; }
        public DateTime? CloseDate { get; set; }
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string StateCode { get; set; }
        public string PostalCode { get; set; }
        public string LegalDescription { get; set; }
        public Guid CommunityId { get; set; }
        public Guid? CurrentHomeOwnerId { get; set; }
        public string CommunityName { get; set; }
        public string PlanType { get; set; }
        public string PlanTypeDescription { get; set; }
        public string PlanName { get; set; }
        public string PlanNumber { get; set; }
        public string Elevation { get; set; }
        public string ProjectName { get; set; }
        public string Swing { get; set; }
        public Guid? BuilderEmployeeId { get; set; }
        public string BuilderName { get; set; }
        public Guid? SalesConsultantEmployeeId { get; set; }
        public string SalesConsultantName { get; set; }
        public DateTime? WarrantyExpirationDate { get; set; }
        public string ServiceRepName { get; set; }
        public bool DoNotContact { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string JdeIdentifier { get; set; }
        public Guid HomeownerId { get; set; }
        public string HomeOwnerName { get; set; }
        public string HomePhone { get; set; }
        public string OtherPhone { get; set; }
        public string WorkNumber { get; set; }
        public string EmailAddress { get; set; }
        public int YearsWithinWarranty { get; set; }
        public DateTime WarrantyStartDate { get; set; }

        public IEnumerable<JobServiceCall> JobServiceCalls { get; set; }
        public IEnumerable<JobSelection> JobSelections { get; set; }
        public IEnumerable<JobPayment> JobPayments { get; set; }
        public IEnumerable<JobNote> JobNotes { get; set; }
        public IEnumerable<Attachment> Attachments { get; set; }
        public IEnumerable<Homeowner> Homeowners { get; set; }
        public IEnumerable<Vendor> Vendors { get; set; }
        public IEnumerable<CostCodeModel> CostCodes { get; set; }
        public IEnumerable<Task> Tasks { get; set; }

        public class JobPayment
        {
            public Guid PaymentId { get; set; }
            public decimal Amount { get; set; }
            public PaymentStatus PaymentStatus { get; set; }
            public BackchargeStatus BackchargeStatus { get; set; }
            public string PaymentStatusDisplayName { get { return PaymentStatus.DisplayName; } }
            public string BackchargeStatusDisplayName { get { return IsBackcharge ? BackchargeStatus.DisplayName : string.Empty; } }
            public string InvoiceNumber { get; set; }
            public DateTime PaymentCreatedDate { get; set; }
            public Guid BackchargeId { get; set; }
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

        public class Vendor : IEquatable<Vendor>
        {
            public Guid VendorId { get; set; }
            public string Name { get; set; }
            public string Number { get; set; }
            public IList<ContactInfoModel> ContactInfo { get; set; }
            public IList<CostCodeModel> CostCodes { get; set; }
            public string CostCodesSeparatedByComma { get; set; }

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

        public AdditionalContactsModel AdditionalContacts { get; set; }

        public int HomeOwnerNumber { get; set; }

        public class JobSelection
        {
            public string OptionNumber { get; set; }
            public string OptionDescription { get; set; }
        }

        public class JobServiceCall
        {
            public Guid ServiceCallId { get; set; }
            public ServiceCallStatus ServiceCallStatus { get; set; }
            public bool CanApprove { get; set; }
            public string AssignedTo { get; set; }
            public string AssignedToEmployeeNumber { get; set; }
            public string JobNumber { get; set; }
            public string Address { get; set; }
            public string CallNumber { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime? CompletionDate { get; set; }
            public string HomeownerName { get; set; }
            public int NumberOfDaysRemaining { get { return ServiceCallCalculator.CalculateNumberOfDaysRemaining(CreatedDate); } }
            public int NumberOfLineItems { get; set; }
            public int DaysOpenedFor { get; set; }
            public int YearsWithinWarranty { get; set; }
            public DateTime WarrantyStartDate { get; set; }
            public string PhoneNumber { get; set; }
            public string EmailAddress { get; set; }
            public bool IsSpecialProject { get; set; }
            public bool IsEscalated { get; set; }
            public DateTime? EscalationDate { get; set; }
            public string EscalationReason { get; set; }
            public string DivisionName { get; set; }
            public string ProjectName { get; set; }
            public string CommunityName { get; set; }
            public decimal? PaymentAmount { get; set; }
            public int PercentComplete
            {
                get { return ServiceCallCalculator.CalculatePercentComplete(NumberOfDaysRemaining); }
            }

            public List<JobServiceCallNote> JobServiceCallNotes { get; set; }

            public string Summary { get; set; }

            public string[] SummaryOfLineItems
            {
                get
                {
                    return Summary != null ? Summary.Split('|') : new string[0];
                }
            }

//NPoco needs prop to be List<> to use FetchOneToMany(). Also need to set to IGNORE if inserting.

            public class JobServiceCallNote
            {
                public Guid ServiceCallNoteId { get; set; }
                public Guid ServiceCallId { get; set; }
                public string Note { get; set; }
            }
        }

        public class JobNote
        {
            public Guid JobNoteId { get; set; }
            public Guid JobId { get; set; }
            public string Note { get; set; }
            public string CreatedBy { get; set; }
            public DateTime? CreatedDate { get; set; }
        }

        public class Attachment
        {
            public Guid JobAttachmentId { get; set; }
            public Guid JobId { get; set; }
            public string DisplayName { get; set; }
            public string FilePath { get; set; }
            public string CreatedBy { get; set; }
            public DateTime? CreatedDate { get; set; }
        }

        public class Homeowner
        {
            public string HomeownerName { get; set; }
            public string CreatedBy { get; set; }
            public DateTime? CreatedDate { get; set; }
        }

        public class Task
        {
            public string EmployeeName { get; set; }
            public string Description { get; set; }
            public TaskType TaskType { get; set; }
            public DateTime? CompletedDate { get; set; }
        }
    }
}
