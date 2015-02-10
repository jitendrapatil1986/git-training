namespace Warranty.Core.Features.NoActionServiceCallLineItem
{
    using System;
    using Enumerations;

    public class NoActionServiceCallLineItemCommand : ICommand<ServiceCallLineItemStatus>
    {
        public Guid ServiceCallLineItemId { get; set; }
    }
}