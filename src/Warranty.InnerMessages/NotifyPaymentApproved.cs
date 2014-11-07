namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyPaymentApproved : ICommand
    {
        public Guid PaymentId { get; set; }
    }
}
