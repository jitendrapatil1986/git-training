namespace Warranty.Core.Features.AddServiceCallLineItem
{
    public class AddServiceCallLineItemCommand : ICommand<bool>
    {
        public AddServiceCallLineItemModel Model { get; set; }
    }
}