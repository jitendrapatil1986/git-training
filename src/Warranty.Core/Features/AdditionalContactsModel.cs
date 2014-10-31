using System.Collections.Generic;

namespace Warranty.Core.Features
{
    using System;
    using Enumerations;

    public class AdditionalContactsModel : ICommand
    {
        public Guid HomeownerId { get; set; }
        public IEnumerable<AdditionalContact> AdditionalPhoneContacts { get; set; }
        public IEnumerable<AdditionalContact> AdditionalEmailContacts { get; set; }

        public class AdditionalContact
        {
            public string ContactValue { get; set; }
            public string ContactLabel { get; set; }
            public Guid HomeownerContactId { get; set; }
            public Guid HomeownerId { get; set; }
            public HomeownerContactType ContactType { get; set; }
        }
    }
}
