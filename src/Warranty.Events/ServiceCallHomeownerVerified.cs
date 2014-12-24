namespace Warranty.Events
{
    using System;
    using NServiceBus;

    public class ServiceCallHomeownerVerified : IEvent
    {
        public Guid ServiceCallId { get; set; }
        public string HomeownerVerificationSignature { get; set; }
        public DateTime? HomeownerVerificationSignatureDate { get; set; }
        public string ServiceCallStatus { get; set; }
    }
}