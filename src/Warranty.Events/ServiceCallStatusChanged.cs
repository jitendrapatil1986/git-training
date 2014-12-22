namespace Warranty.Events
{
    using System;
    using NServiceBus;

    public class ServiceCallStatusChanged : IEvent
    {
        public Guid ServiceCallId { get; set; }
        public string StatusDisplayName { get; set; }
    }
}