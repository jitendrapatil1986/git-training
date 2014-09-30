namespace Warranty.Core.Features.UpdateServiceCallLineItem
{
    using System;
    using Enumerations;

    public class UpdateServiceCallLineItemCommand : ICommand<ServiceCallLineItemStatus>
    {
        public Guid ServiceCallLineItemId { get; set; }
    }
}