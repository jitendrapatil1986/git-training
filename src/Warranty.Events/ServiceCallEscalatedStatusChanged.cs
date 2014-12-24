namespace Warranty.Events
{
    using System;
    using NServiceBus;

    public class ServiceCallEscalatedStatusChanged : IEvent
    {
        public Guid ServiceCallId { get; set; }
        public bool Escalated { get; set; }
        public string EscalationReason { get; set; }
        public DateTime? EscalationDate { get; set; }
    }
}