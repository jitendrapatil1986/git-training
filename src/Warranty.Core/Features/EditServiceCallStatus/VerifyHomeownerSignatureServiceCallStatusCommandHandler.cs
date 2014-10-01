namespace Warranty.Core.Features.EditServiceCallStatus
{
    using Entities;
    using Enumerations;
    using NPoco;

    public class VerifyHomeownerSignatureServiceCallStatusCommandHandler : ICommandHandler<VerifyHomeownerSignatureServiceCallStatusCommand, VerifyHomeownerSignatureServiceCallStatusModel>
    {
        private readonly IDatabase _database;

        public VerifyHomeownerSignatureServiceCallStatusCommandHandler(IDatabase database)
        {
            _database = database;
        }

        public VerifyHomeownerSignatureServiceCallStatusModel Handle(VerifyHomeownerSignatureServiceCallStatusCommand message)
        {
            using (_database)
            {
                var updateServiceCall = _database.SingleById<ServiceCall>(message.ServiceCallId);
                updateServiceCall.HomeownerVerificationSignature = message.HomeownerVerificationSignature;
                updateServiceCall.HomeownerVerificationSignatureDate = message.HomeownerVerificationSignatureDate;

                if (updateServiceCall.ServiceCallStatus != ServiceCallStatus.Complete)
                    updateServiceCall.ServiceCallStatus = ServiceCallStatus.HomeownerSigned;

                _database.Update(updateServiceCall);

                var model = new VerifyHomeownerSignatureServiceCallStatusModel
                    {
                        ServiceCallId = updateServiceCall.ServiceCallId,
                        ServiceCallStatus = updateServiceCall.ServiceCallStatus,
                        HomeownerVerificationSignature = updateServiceCall.HomeownerVerificationSignature,
                        HomeownerVerificationSignatureDate = updateServiceCall.HomeownerVerificationSignatureDate,
                    };

                return model;
            }
        }
    }
}