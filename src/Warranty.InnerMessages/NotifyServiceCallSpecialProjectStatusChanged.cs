namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyServiceCallSpecialProjectStatusChanged : ICommand
    {
        public Guid ServiceCallId { get; set; }
        public DateTime? SpecialProjectDate { get; set; }
        public string SpecialProjectReason { get; set; }
    }
}