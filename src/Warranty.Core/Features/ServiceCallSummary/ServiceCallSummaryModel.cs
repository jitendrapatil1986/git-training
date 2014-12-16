using System;
using System.Collections.Generic;
using Warranty.Core.Enumerations;

namespace Warranty.Core.Features.ServiceCallSummary
{
    using System.Web.Mvc;
    using Services;

    public class ServiceCallSummaryModel : UploadAttachmentBaseViewModel
    {
        public ServiceCall ServiceCallSummary { get; set; }
        public IEnumerable<ServiceCallLine> ServiceCallLines { get; set; }
        public IEnumerable<ServiceCallNote> ServicCallNotes { get; set; }
        public IEnumerable<Attachment> Attachments { get; set; }
        public NewServiceCallLineItem AddServiceCallLineItem { get; set; }
        public AdditionalContactsModel AdditionalContacts { get; set; }
        public bool CanApprove { get; set; }
        public bool CanReassign { get; set; }
        public bool CanReopenLines { get; set; }

        public class NewServiceCallLineItem
        {
            public NewServiceCallLineItem(Guid serviceCallId, IEnumerable<SelectListItem> problemCodes)
            {
                ServiceCallId = serviceCallId;
                ProblemCodes = problemCodes;
            }

            public NewServiceCallLineItem()
            {
            }

            public Guid ServiceCallId { get; set; }
            public IEnumerable<SelectListItem> ProblemCodes { get; set; }
            public IEnumerable<SelectListItem> RootCauses { get; set; } 
        }

        public class ServiceCall
        {
            public Guid ServiceCallId { get; set; }
            public string AssignedTo { get; set; }
            public string AssignedToEmployeeNumber { get; set; }
            public Guid JobId { get; set; }
            public string JobNumber { get; set; }
            public string Address { get; set; }
            public string CallNumber { get; set; }
            public DateTime CreatedDate { get; set; }
            public string CreatedBy { get; set; }
            public DateTime? CompletionDate { get; set; }
            public string HomeownerName { get; set; }
            public Guid HomeownerId { get; set; }
            public int HomeownerNumber { get; set; }
            public int NumberOfDaysRemaining { get { return ServiceCallCalculator.CalculateNumberOfDaysRemaining(CreatedDate); } }
            public int NumberOfLineItems { get; set; }
            public int DaysOpenedFor { get; set; }
            public int YearsWithinWarranty { get; set; }
            public DateTime WarrantyStartDate { get; set; }
            public string HomePhone { get; set; }
            public string OtherPhone { get; set; }
            public string EmailAddress { get; set; }
            public bool IsSpecialProject { get; set; }
            public bool IsEscalated { get; set; }
            public bool CanBeReopened { get; set; }
            public DateTime? EscalationDate { get; set; }
            public string EscalationReason { get; set; }
            public string DivisionName { get; set; }
            public string ProjectName { get; set; }
            public string CommunityName { get; set; }
            public decimal? PaymentAmount { get; set; }
            public ServiceCallStatus ServiceCallStatus { get; set; }
            public string HomeOwnerSignature { get; set; }
            public string HomeownerVerificationSignature { get; set; }
            public DateTime? HomeownerVerificationSignatureDate { get; set; }
            public HomeownerVerificationType HomeownerVerificationType { get; set; }
            public string SpecialProjectReason { get; set; }
            public DateTime? SpecialProjectDate { get; set; }

            public int PercentComplete
            {
                get { return ServiceCallCalculator.CalculatePercentComplete(NumberOfDaysRemaining); }
            }
        }

        public class ServiceCallLine
        {
            public Guid ServiceCallLineItemId { get; set; }
            public Guid ServiceCallId { get; set; }
            public int LineNumber { get; set; }
            public string ProblemCode { get; set; }
            public string ProblemDetailCode { get; set; }
            public string ProblemJdeCode { get; set; }
            public string ProblemDescription { get; set; }
            public string CauseDescription { get; set; }
            public string ClassificationNote { get; set; }
            public string LineItemRoot { get; set; }
            public DateTime CreatedDate { get; set; }
            public ServiceCallLineItemStatus ServiceCallLineItemStatus { get; set; }
            public int NumberOfAttachments { get; set; }
            public int NumberOfNotes { get; set; }
        }

        public class ServiceCallNote
        {
            public Guid ServiceCallNoteId { get; set; }
            public Guid ServiceCallId { get; set; }
            public string Note { get; set; }
            public Guid ServiceCallLineItemId { get; set; }
            public DateTime? CreatedDate { get; set; }
            public string CreatedBy { get; set; }
        }

        public class Attachment
        {
            public Guid ServiceCallId { get; set; }
            public Guid ServiceCallAttachmentId { get; set; }
            public Guid ServiceCallLineItemId { get; set; }
            public string DisplayName { get; set; }
            public DateTime? CreatedDate { get; set; }
            public string CreatedBy { get; set; }
        }

        public class EmployeeViewModel
        {
            public string EmployeeNumber { get; set; }
            public string DisplayName { get; set; }
        }
    }
}
