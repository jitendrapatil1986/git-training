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
        public string WorkSummary { get; set; }
    }
}
