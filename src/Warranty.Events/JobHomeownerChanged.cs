namespace Warranty.Events
{
    using System;
    using NServiceBus;

    public class JobHomeownerChanged : IEvent
    {
        public Guid JobId { get; set; }
        public string HomeownerName { get; set; }
        public string HomeownerHomePhone { get; set; }
        public string HomeownerEmailAddress { get; set; }
    }
}