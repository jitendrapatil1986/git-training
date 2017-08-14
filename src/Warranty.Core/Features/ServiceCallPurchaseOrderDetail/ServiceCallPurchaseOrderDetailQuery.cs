namespace Warranty.Core.Features.ServiceCallPurchaseOrderDetail
{
    using System;

    public class ServiceCallPurchaseOrderDetailQuery : IQuery<ServiceCallPurchaseOrderDetailModel>
    {
        public Guid ServiceCallLineItemId { get; set; }
        public Guid PurchaseOrderId { get; set; }
    }
}