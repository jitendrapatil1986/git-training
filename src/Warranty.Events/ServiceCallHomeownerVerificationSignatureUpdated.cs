﻿namespace Warranty.Events
{
    using System;
    using NServiceBus;

    public class ServiceCallHomeownerVerificationSignatureUpdated : IEvent
    {
        public int ServiceCallNumber { get; set; }
        public string HomeownerVerificationSignature { get; set; }
        public DateTime? HomeownerVerificationSignatureDate { get; set; }
        public string ServiceCallStatus { get; set; }
    }
}