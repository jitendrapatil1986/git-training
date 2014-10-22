namespace Warranty.Core.Features.EditServiceCallStatus
{
    using Entities;
    using Enumerations;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class VerifyHomeownerSignatureServiceCallStatusCommandHandler : ICommandHandler<VerifyHomeownerSignatureServiceCallStatusCommand, VerifyHomeownerSignatureServiceCallStatusModel>
    {
        private readonly IDatabase _database;
        private readonly IBus _bus;

        public VerifyHomeownerSignatureServiceCallStatusCommandHandler(IDatabase database, IBus bus)
        {
            _database = database;
            _bus = bus;
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
                
                _bus.Send<NotifyServiceCallHomeownerVerified>(x =>
                    {
                        x.ServiceCallId = updateServiceCall.ServiceCallId;
                    });

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