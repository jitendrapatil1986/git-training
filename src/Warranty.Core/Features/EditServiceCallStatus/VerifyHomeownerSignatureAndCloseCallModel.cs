namespace Warranty.Core.Features.EditServiceCallStatus
{
    using System;
    using Enumerations;

    public class VerifyHomeownerSignatureAndCloseCallModel
    {
        public Guid ServiceCallId { get; set; }
        public ServiceCallStatus ServiceCallStatus { get; set; }
        public string HomeownerVerificationSignature { get; set; }
        public DateTime? HomeownerVerificationSignatureDate { get; set; }
        public HomeownerVerificationType HomeownerVerificationType { get; set; }
    }
}