namespace Warranty.Events
{
    using System;
    using NServiceBus;

    public class ServiceCallEmployeeReassigned : IEvent
    {
        public Guid ServiceCallId { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
    }
}