namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyServiceCallCreated : ICommand
    {
        public Guid ServiceCallId { get; set; }
    }
}
