namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyServiceCallHomeownerVerified : ICommand
    {
        public Guid ServiceCallId { get; set; }
    }
}