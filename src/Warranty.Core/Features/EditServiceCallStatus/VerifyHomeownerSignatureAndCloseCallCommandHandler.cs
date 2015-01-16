namespace Warranty.Core.Features.EditServiceCallStatus
{
    using System;
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class VerifyHomeownerSignatureAndCloseCallCommandHandler : ICommandHandler<VerifyHomeownerSignatureAndCloseCallCommand, VerifyHomeownerSignatureAndCloseCallModel>
    {
        private readonly IDatabase _database;
        private readonly IBus _bus;
        private readonly IActivityLogger _logger;

        public VerifyHomeownerSignatureAndCloseCallCommandHandler(IDatabase database, IBus bus, IActivityLogger logger)
        {
            _database = database;
            _bus = bus;
            _logger = logger;
        }

        public VerifyHomeownerSignatureAndCloseCallModel Handle(VerifyHomeownerSignatureAndCloseCallCommand message)
        {
            using (_database)
            {
                var updateServiceCall = _database.SingleById<ServiceCall>(message.ServiceCallId);
                updateServiceCall.HomeownerVerificationSignature = message.HomeownerVerificationSignature;
                updateServiceCall.HomeownerVerificationSignatureDate = message.HomeownerVerificationSignatureDate;
                updateServiceCall.HomeownerVerificationType = HomeownerVerificationType.FromValue(message.HomeownerVerificationTypeId);
                updateServiceCall.ServiceCallStatus = ServiceCallStatus.Complete;
                updateServiceCall.CompletionDate = DateTime.UtcNow;

                _database.Update(updateServiceCall);
                
                _bus.Send<NotifyServiceCallHomeownerVerified>(x =>
                    {
                        x.ServiceCallId = updateServiceCall.ServiceCallId;
                    });

                _bus.Send<NotifyServiceCallCompleted>(x =>
                {
                    x.ServiceCallId = updateServiceCall.ServiceCallId;
                });

                const string activityName = "Service call was completed.";

                _logger.Write(activityName, null, message.ServiceCallId, ActivityType.Complete, ReferenceType.ServiceCall);

                var model = new VerifyHomeownerSignatureAndCloseCallModel
                    {
                        ServiceCallId = updateServiceCall.ServiceCallId,
                        ServiceCallStatus = updateServiceCall.ServiceCallStatus,
                        HomeownerVerificationSignature = updateServiceCall.HomeownerVerificationSignature,
                        HomeownerVerificationSignatureDate = updateServiceCall.HomeownerVerificationSignatureDate,
                        HomeownerVerificationType = updateServiceCall.HomeownerVerificationType,
                    };

                return model;
            }
        }
    }
}