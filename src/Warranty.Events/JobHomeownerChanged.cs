namespace Warranty.Events
{
    using NServiceBus;

    public class JobHomeownerChanged : IEvent
    {
        public string JobNumber { get; set; }
        public string HomeownerName { get; set; }
        public string HomeownerHomePhone { get; set; }
        public string HomeownerEmailAddress { get; set; }
    }
}