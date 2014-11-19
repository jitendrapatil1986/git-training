namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyPurchaseOrderRequested : ICommand
    {
        public Guid PurchaseOrderId { get; set; }
        public string LoginName { get; set; }
        public string JobNumber { get; set; }
    }
}