namespace Warranty.Events
{
    using System;
    using NServiceBus;

    public class RequestServiceCallResponse : IEvent
    {
         public string LocalId { get; set; }
         public Guid ServiceCallId { get; set; }
    }
}