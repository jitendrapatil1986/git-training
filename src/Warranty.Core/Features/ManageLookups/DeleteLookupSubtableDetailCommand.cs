namespace Warranty.Core.Features.ManageLookups
{
    public class DeleteLookupSubtableDetailCommand : ICommand<bool>
    {
        public DeleteLookupSubtableDetailModel Model { get; set; }
    }
}
