namespace Warranty.Core.Features.UpdateServiceCallLineItem
{
    using System;
    using Enumerations;

    public class UpdateServiceCallLineItemCommand : ICommand<UpdateServiceCallLineItemModel>
    {
        public Guid ServiceCallLineItemId { get; set; }
    }
}