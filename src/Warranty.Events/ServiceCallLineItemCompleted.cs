namespace Warranty.Events
{
    using System;
    using NServiceBus;

    public class ServiceCallLineItemCompleted : IEvent
    {
        public Guid ServiceCallId { get; set; }
        public Guid ServiceCallLineItemId { get; set; }
        public string ServiceCallLineItemStatus { get; set; }
    }
}