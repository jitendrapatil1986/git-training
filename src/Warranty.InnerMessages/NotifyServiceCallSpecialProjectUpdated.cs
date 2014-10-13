namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyServiceCallSpecialProjectUpdated : ICommand
    {
        public Guid ServiceCallId { get; set; }
        public DateTime SpecialProjectDate { get; set; }
        public string SpecialProjectReason { get; set; }
    }
}