namespace Warranty.Core.Features.ServiceCallSummary.ServiceCallLineItem
{
    using System;

    public class ServiceCallLineItemQuery : IQuery<ServiceCallLineItemModel>
    {
        public Guid ServiceCallLineItemId { get; set; }
    }
}