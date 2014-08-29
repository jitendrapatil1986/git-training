namespace Warranty.Core.Features.QuickSearch
{
    using System;

    public class QuickSearchJobModel
    {
        public Guid Id { get; set; }
        public string JobNumber { get; set; }
        public string AddressLine { get; set; }
        public string HomeOwnerName { get; set; }
        public string HomePhone { get; set; }
    }
}
