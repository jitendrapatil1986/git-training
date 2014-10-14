namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyServiceCallLineItemCompleted : ICommand
    {
        public Guid ServiceCallLineItemId { get; set; }
    }
}