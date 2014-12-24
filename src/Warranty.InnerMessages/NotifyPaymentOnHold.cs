namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyPaymentOnHold : ICommand
    {
        public Guid PaymentId { get; set; }
        public string Username { get; set; }
    }
}
