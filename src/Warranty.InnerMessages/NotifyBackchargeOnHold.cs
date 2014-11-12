namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyBackchargeOnHold : ICommand
    {
        public Guid BackchargeId { get; set; }
    }
}
