namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyRequestedPayment  : ICommand
    {
        public Guid PaymentId { get; set; }
    }
}