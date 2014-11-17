namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyBackchargeDeny : ICommand
    {
        public Guid BackchargeId { get; set; }
        public string UserName { get; set; }
    }
}
