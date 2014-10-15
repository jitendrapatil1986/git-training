namespace Warranty.Events
{
    using System;
    using System.Collections.Generic;
    using NServiceBus;

    public class ServiceCallCreated : IEvent
    {
        public Guid ServiceCallId { get; set; }
        public int ServiceCallNumber { get; set; }
        public string ServiceCallType { get; set; }
        public string ServiceCallStatus { get; set; }
        public string JobNumber { get; set; }
        public string Contact { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public string WorkSummary { get; set; }

        public List<ServiceCallLineItem> ServiceCallLineItems { get; set; }

        public class ServiceCallLineItem
        {
            public Guid ServiceCallLineItemId { get; set; }
            public Guid ServiceCallId { get; set; }
            public int LineNumber { get; set; }
            public string ProblemCode { get; set; }
            public string ProblemDescription { get; set; }
            public string CauseDescription { get; set; }
            public string ClassificationNote { get; set; }
            public string LineItemRoot { get; set; }
            public string ServiceCallLineItemStatus { get; set; }
        }
    }
}
