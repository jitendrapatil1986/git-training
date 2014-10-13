namespace Warranty.Events
{
    using System;
    using NServiceBus;

    public class ServiceCallSpecialProjectUpdated : IEvent
    {
        public int ServiceCallNumber { get; set; }
        public bool SpecialProject { get; set; }
        public string SpecialProjectReason { get; set; }
        public DateTime? SpecialProjectDate { get; set; }
    }
}