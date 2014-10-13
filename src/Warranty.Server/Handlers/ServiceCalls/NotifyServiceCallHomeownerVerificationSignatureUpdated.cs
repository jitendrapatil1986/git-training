namespace Warranty.Server.Handlers.ServiceCalls
{
    using Core.Entities;
    using Events;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyServiceCallHomeownerVerificationSignatureUpdatedHandler : IHandleMessages<NotifyServiceCallHomeownerVerificationSignatureUpdated>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;

        public NotifyServiceCallHomeownerVerificationSignatureUpdatedHandler(IBus bus, IDatabase database)
        {
            _bus = bus;
            _database = database;
        }

        public void Handle(NotifyServiceCallHomeownerVerificationSignatureUpdated message)
        {
            using (_database)
            {
                var serviceCall = _database.SingleById<ServiceCall>(message.ServiceCallId);

                _bus.Publish<ServiceCallHomeownerVerificationSignatureUpdated>(x =>
                {
                    x.ServiceCallNumber = serviceCall.ServiceCallNumber;
                    x.HomeownerVerificationSignature = serviceCall.HomeownerVerificationSignature;
                    x.HomeownerVerificationSignatureDate = serviceCall.HomeownerVerificationSignatureDate;
                });
            }
        }
    }
}