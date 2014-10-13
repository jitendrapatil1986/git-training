namespace Warranty.Events
{
    using System;
    using NServiceBus;

    public class ServiceCallCompletionUpdated : IEvent
    {
        public int ServiceCallNumber { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string ServiceCallStatus { get; set; }
    }
}