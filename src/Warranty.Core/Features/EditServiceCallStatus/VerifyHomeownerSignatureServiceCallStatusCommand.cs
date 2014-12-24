namespace Warranty.Core.Features.EditServiceCallStatus
{
    using System;
    using Enumerations;

    public class VerifyHomeownerSignatureServiceCallStatusCommand : ICommand<VerifyHomeownerSignatureServiceCallStatusModel>
    {
        public Guid ServiceCallId { get; set; }
        public ServiceCallStatus ServiceCallStatus { get; set; }
        public string HomeownerVerificationSignature { get; set; }
        public DateTime? HomeownerVerificationSignatureDate { get; set; }
        public int HomeownerVerificationTypeId { get; set; }
    }
}