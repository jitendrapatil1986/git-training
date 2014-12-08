namespace Warranty.Core.Features.UpdateServiceCallLineItem
{
    using System;
    using Enumerations;

    public class UpdateServiceCallLineItemModel
    {
        public Guid ServiceCallLineItemId { get; set; }
        public ServiceCallLineItemStatus ServiceCallLineItemStatus { get; set; }
        public ServiceCallStatus ServiceCallStatus { get; set; }
    }
}