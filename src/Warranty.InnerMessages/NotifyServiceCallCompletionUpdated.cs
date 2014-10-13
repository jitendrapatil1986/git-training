namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyServiceCallCompletionUpdated : ICommand
    {
        public Guid ServiceCallId { get; set; }
    }
}