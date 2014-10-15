namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyServiceCallLineItemProblemChanged : ICommand
    {
        public Guid ServiceCallLineItemId { get; set; }
    }
}