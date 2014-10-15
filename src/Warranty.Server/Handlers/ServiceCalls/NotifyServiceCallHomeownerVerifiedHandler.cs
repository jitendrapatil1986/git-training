namespace Warranty.Server.Handlers.ServiceCalls
{
    using Core.Entities;
    using Events;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyServiceCallHomeownerVerifiedHandler : IHandleMessages<NotifyServiceCallHomeownerVerified>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;

        public NotifyServiceCallHomeownerVerifiedHandler(IBus bus, IDatabase database)
        {
            _bus = bus;
            _database = database;
        }

        public void Handle(NotifyServiceCallHomeownerVerified message)
        {
            using (_database)
            {
                var serviceCall = _database.SingleById<ServiceCall>(message.ServiceCallId);

                _bus.Publish<ServiceCallHomeownerVerified>(x =>
                {
                    x.ServiceCallNumber = serviceCall.ServiceCallNumber;
                    x.HomeownerVerificationSignature = serviceCall.HomeownerVerificationSignature;
                    x.HomeownerVerificationSignatureDate = serviceCall.HomeownerVerificationSignatureDate;
                });
            }
        }
    }
}