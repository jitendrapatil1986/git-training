namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyServiceCallEmployeeUpdated : ICommand
    {
        public Guid ServiceCallId { get; set; }
    }
}