namespace Warranty.Core.Features.QuickSearch
{
    using System;

    public class QuickSearchCallModel
    {
        public Guid Id { get; set; }
        public string ProblemCodes { get; set; }
        public string JobNumber { get; set; }
        public string AddressLine { get; set; }
        public string HomeOwnerName { get; set; }
        public string HomePhone { get; set; }
    }
}