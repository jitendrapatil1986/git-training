namespace Warranty.Events
{
    using System;
    using NServiceBus;

    public class ServiceCallEscalatedUpdated : IEvent
    {
        public int ServiceCallNumber { get; set; }
        public bool Escalated { get; set; }
        public string EscalationReason { get; set; }
        public DateTime? EscalationDate { get; set; }
    }
}