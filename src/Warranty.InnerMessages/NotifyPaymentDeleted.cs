namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyPaymentDeleted : ICommand
    {
        public Guid PaymentId { get; set; }
        public string UserName { get; set; }
    }
}