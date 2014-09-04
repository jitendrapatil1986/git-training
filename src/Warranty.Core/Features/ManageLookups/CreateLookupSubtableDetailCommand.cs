namespace Warranty.Core.Features.ManageLookups
{
    public class CreateLookupSubtableDetailCommand : ICommand<bool>
    {
        public CreateLookupSubtableDetailModel Model { get; set; }
    }
}
