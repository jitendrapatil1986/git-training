namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyServiceCallLineItemCompletionUpdated : ICommand
    {
        public Guid ServiceCallLineItemId { get; set; }
    }
}