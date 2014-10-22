namespace Warranty.Events
{
    using System;
    using System.Collections.Generic;
    using NServiceBus;

    public class JobHomeownerChanged : IEvent
    {
        public Guid JobId { get; set; }
        public int? HomeownerNumber { get; set; }
        public string HomeownerName { get; set; }
        public string HomeownerHomePhone { get; set; }
        public string HomeownerEmailAddress { get; set; }

        public List<AdditionalPhoneContact> AdditionalPhoneContacts { get; set; }

        public class AdditionalPhoneContact
        {
            public string ContactType { get; set; }
            public string ContactValue { get; set; }
        }

        public List<AdditionalEmailContact> AdditionalEmailContacts { get; set; }

        public class AdditionalEmailContact
        {
            public string ContactType { get; set; }
            public string ContactValue { get; set; }
        }
    }
}