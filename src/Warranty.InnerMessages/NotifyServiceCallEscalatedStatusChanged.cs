namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyServiceCallEscalatedStatusChanged : ICommand
    {
        public Guid ServiceCallId { get; set; }
        public DateTime? EscalatedDate { get; set; }
        public string EscalatedReason { get; set; }
    }
}