namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyPaymentRequested  : ICommand
    {
        public Guid PaymentId { get; set; }
        public string Username { get; set; }
    }
}