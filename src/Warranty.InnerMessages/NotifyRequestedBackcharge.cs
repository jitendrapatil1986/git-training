namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyRequestedBackcharge : ICommand
    {
        public Guid BackchargeId { get; set; }
    }
}