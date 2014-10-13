namespace Warranty.Events
{
    using System;
    using NServiceBus;

    public class ServiceCallCreated : IEvent
    {
        public int ServiceCallNumber { get; set; }
        public string ServiceCallType { get; set; }
        public string ServiceCallStatus { get; set; }
        public string JobNumber { get; set; }
        public string Contact { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string WorkSummary { get; set; }
        public string HomeownerSignature { get; set; }
        public bool SpecialProject { get; set; }
        public bool Escalated { get; set; }
        public DateTime? EscalationDate { get; set; }
        public string EscalationReason { get; set; }
        public string HomeownerVerificationSignature { get; set; }
        public DateTime? HomeownerVerificationSignatureDate { get; set; }
    }
}
