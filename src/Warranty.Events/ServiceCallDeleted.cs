namespace Warranty.Events
{
    using System;
    using NServiceBus;

    public class ServiceCallDeleted : IEvent
    {
        public Guid ServiceCallId { get; set; }
    }
}