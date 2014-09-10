namespace Warranty.Core.Features.ManageLookups
{
    public class DeleteLookupItemCommand : ICommand<bool>
    {
        public int Id { get; set; }
        public string LookupType { get; set; }
    }
}
