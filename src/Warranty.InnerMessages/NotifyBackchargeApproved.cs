namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyBackchargeApproved : ICommand
    {
        public Guid BackchargeId { get; set; }
        public string UserName { get; set; }
    }
}
