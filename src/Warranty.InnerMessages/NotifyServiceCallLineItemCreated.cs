namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyServiceCallLineItemCreated : ICommand
    {
        public Guid ServiceCallLineItemId { get; set; }
    }
}