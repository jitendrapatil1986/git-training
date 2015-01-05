namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyServiceCallStatusChanged : ICommand
    {
        public Guid ServiceCallId { get; set; }
    }
}