namespace Warranty.Core.Features.AddServiceCallPurchaseOrder
{
    using System;

    public class AddServiceCallPurchaseOrderQuery : IQuery<AddServiceCallPurchaseOrderModel>
    {
        public Guid ServiceCallLineItemId { get; set; }
    }
}