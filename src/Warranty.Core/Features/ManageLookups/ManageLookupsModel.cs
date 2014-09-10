namespace Warranty.Core.Features.ManageLookups
{
    using System.Collections.Generic;

    public class ManageLookupsModel : ICommand<int>
    {
        public ManageLookupsModel()
        {
            LookupTypes = new List<string>();
        }

        public IEnumerable<string> LookupTypes { get; set; }
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string LookupType { get; set; }
    }
}
