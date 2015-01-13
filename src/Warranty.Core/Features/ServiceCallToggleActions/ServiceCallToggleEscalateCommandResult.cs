namespace Warranty.Core.Features.ServiceCallToggleActions
{
    using System;
    using System.Collections.Generic;

    public class ServiceCallToggleEscalateCommandResult 
    {
        public ServiceCallToggleEscalateCommandResult()
        {
            Emails = new List<string>(0);
        }

        public Guid ServiceCallId { get; set; }
        public int CallNumber { get; set; }
        public string HomeOwnerName { get; set; }
        public string HomePhone { get; set; }
        public string CommunityName { get; set; }
        public string AddressLine { get; set; }
        public bool IsEscalated { get; set; }
        public IEnumerable<string> Emails { get; set; }
        public bool ShouldSendEmail { get; set; }
        public string Url { get; set; }
        public List<string> Comments { get; set; }
    }
}