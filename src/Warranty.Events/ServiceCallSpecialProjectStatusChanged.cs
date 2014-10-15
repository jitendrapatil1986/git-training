namespace Warranty.Events
{
    using System;
    using NServiceBus;

    public class ServiceCallSpecialProjectStatusChanged : IEvent
    {
        public Guid ServiceCallId { get; set; }
        public bool SpecialProject { get; set; }
        public string SpecialProjectReason { get; set; }
        public DateTime? SpecialProjectDate { get; set; }
    }
}