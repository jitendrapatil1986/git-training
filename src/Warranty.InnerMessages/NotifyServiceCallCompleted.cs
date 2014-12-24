namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyServiceCallCompleted : ICommand
    {
        public Guid ServiceCallId { get; set; }
    }
}