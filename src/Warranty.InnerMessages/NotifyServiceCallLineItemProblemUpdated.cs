namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyServiceCallLineItemProblemUpdated : ICommand
    {
        public Guid ServiceCallLineItemId { get; set; }
    }
}