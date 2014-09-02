namespace Warranty.Core.Features.ManageLookups
{
    using System.Collections.Generic;

    public class ManageLookupsModel
    {
        public ManageLookupsModel()
        {
            LookupTypes = new List<string>();
        }

        public IEnumerable<string> LookupTypes { get; set; }
    }
}
