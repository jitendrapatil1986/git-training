namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyServiceCallDeleted : ICommand
    {
        public Guid ServiceCallId { get; set; }
    }
}