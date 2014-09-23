namespace Warranty.Core.Features.QuickSearch
{
    using System;
    using Extensions;

    public class QuickSearchJobModel
    {
        public Guid Id { get; set; }
        public string JobNumber { get; set; }
        public string AddressLine { get; set; }
        public string HomeOwnerName { get; set; }
        public string HomePhone { get; set; }
        public string EmailAddress { get; set; }
        public int HomeOwnerNumber { get; set; }
        public string HomeOwnerNumberWithSuffix {get { return HomeOwnerNumber.AddOrdinalSuffix(); }}
        public bool NotFirstHomeOwner{get { return HomeOwnerNumber > 1; }}
    }
}
