using System;
using System.Collections.Generic;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;
using Warranty.Core.Services;

namespace Warranty.Core.Features.Report
{
    public class ServiceCall
    {
        public Guid ServiceCallId { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string ServiceCallNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public string HomeownerName { get; set; }
        public Guid CommunityId { get; set; }
        public string CommunityName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string StateCode { get; set; }
        public string PostalCode { get; set; }
        public string HomePhone { get; set; }
        public string OtherPhone { get; set; }
        public bool IsSpecialProject { get; set; }
        public string JobNumber { get; set; }
        public Guid JobId { get; set; }
        public int NumberOfDaysRemaining { get { return ServiceCallCalculator.CalculateNumberOfDaysRemaining(CreatedDate); } }
        public List<ServiceCallLine> ServiceCallLines { get; set; }
        public IEnumerable<ServiceCallNote> ServiceCallNotes { get; set; }
    }

    public class ServiceCallLine
    {
        public Guid ServiceCallId { get; set; }
        public int LineNumber { get; set; }
        public string ProblemCode { get; set; }
        public string ProblemDescription { get; set; }
        public ServiceCallLineItemStatus ServiceCallLineItemStatus { get; set; }
    }

    public class ServiceCallNote
    {
        public Guid ServiceCallId { get; set; }
        public string Note { get; set; }
    }
}