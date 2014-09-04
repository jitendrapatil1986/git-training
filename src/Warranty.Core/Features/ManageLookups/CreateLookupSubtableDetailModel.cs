namespace Warranty.Core.Features.ManageLookups
{
    public class CreateLookupSubtableDetailModel : ICommand<int>
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string LookupType { get; set; }
    }
}
