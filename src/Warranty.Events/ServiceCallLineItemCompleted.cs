namespace Warranty.Events
{
    using System;
    using NServiceBus;

    public class ServiceCallLineItemCompleted : IEvent
    {
        public int ServiceCallNumber { get; set; }
        public int LineNumber { get; set; }
        public string ServiceCallLineItemStatus { get; set; }
    }
}