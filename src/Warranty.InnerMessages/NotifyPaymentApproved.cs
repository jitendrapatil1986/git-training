namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyPaymentApproved : ICommand
    {
        public Guid PaymentId { get; set; }
        public string UserName { get; set; }
    }
}
