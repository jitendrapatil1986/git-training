namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyBackchargeDeleted : ICommand
    {
        public Guid BackchargeId { get; set; } 
    }
}