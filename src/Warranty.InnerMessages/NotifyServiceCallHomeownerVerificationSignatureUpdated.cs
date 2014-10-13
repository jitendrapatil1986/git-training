namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyServiceCallHomeownerVerificationSignatureUpdated : ICommand
    {
        public Guid ServiceCallId { get; set; }
    }
}