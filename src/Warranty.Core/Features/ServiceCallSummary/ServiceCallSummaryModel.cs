﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warranty.Core.Features.ServiceCallSummary
{
    using Services;

    public class ServiceCallSummaryModel
    {
        public ServiceCall ServiceCallSummary { get; set; }
        public IEnumerable<ServiceCallLine> ServiceCallLines { get; set; }
        public IEnumerable<ServicCallComment> ServicCallComments { get; set; }

        public class ServiceCall
        {
            public Guid ServiceCallId { get; set; }
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
        }

        public class ServiceCallLine
        {
            public Guid ServiceCallLineItemId { get; set; }
            public Guid ServiceCallId { get; set; }
            public int LineNumber { get; set; }
            public string ProblemCode { get; set; }
            public string ProblemDescription { get; set; }
            public string CauseDescription { get; set; }
            public string ClassificationNote { get; set; }
            public string LineItemRoot { get; set; }
            public bool Completed { get; set; }
            public DateTime CreatedDate { get; set; }
        }

        public class ServicCallComment
        {
            public Guid ServiceCallCommentId { get; set; }
            public Guid ServiceCallId { get; set; }
            public string Comment { get; set; }
            public DateTime? CreatedDate { get; set; }
            public string CreatedBy { get; set; }
        }
    }
}
