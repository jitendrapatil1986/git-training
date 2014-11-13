namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyBackchargeRequested : ICommand
    {
        public Guid BackchargeId { get; set; }
        public string Username { get; set; }
    }
}