namespace Warranty.Events
{
    using System;
    using NServiceBus;

    public class ServiceCallCompleted : IEvent
    {
        public Guid ServiceCallId { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string ServiceCallStatus { get; set; }
    }
}