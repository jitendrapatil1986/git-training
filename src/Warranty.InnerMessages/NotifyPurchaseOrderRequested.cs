namespace Warranty.InnerMessages
{
    using System;
    using System.Collections.Generic;
    using NServiceBus;

    public class NotifyPurchaseOrderRequested : ICommand
    {
        public Guid PurchaseOrderId { get; set; }
        public string LoginName { get; set; }
        public string CommunityNumber { get; set; }
        public string EmployeeNumber { get; set; }
    }
}