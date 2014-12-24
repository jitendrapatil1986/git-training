namespace Warranty.Core.Features.CompleteServiceCallLineItem
{
    using System;
    using Enumerations;

    public class CompleteServiceCallLineItemCommand : ICommand<ServiceCallLineItemStatus>
    {
        public Guid ServiceCallLineItemId { get; set; } 
    }
}