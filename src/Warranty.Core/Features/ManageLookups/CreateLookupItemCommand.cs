namespace Warranty.Core.Features.ManageLookups
{
    public class CreateLookupItemCommand : ICommand<int>
    {
        public string DisplayName { get; set; }
        public string LookupType { get; set; }
    }
}
