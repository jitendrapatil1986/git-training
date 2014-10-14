namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyServiceCallEmployeeReassigned : ICommand
    {
        public Guid ServiceCallId { get; set; }
    }
}