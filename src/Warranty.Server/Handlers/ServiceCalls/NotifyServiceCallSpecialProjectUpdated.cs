namespace Warranty.Server.Handlers.ServiceCalls
{
    using Core.Entities;
    using Events;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyServiceCallSpecialProjectUpdatedHandler : IHandleMessages<NotifyServiceCallSpecialProjectUpdated>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;

        public NotifyServiceCallSpecialProjectUpdatedHandler(IBus bus, IDatabase database)
        {
            _bus = bus;
            _database = database;
        }

        public void Handle(NotifyServiceCallSpecialProjectUpdated message)
        {
            using (_database)
            {
                var serviceCall = _database.SingleById<ServiceCall>(message.ServiceCallId);

                _bus.Publish<ServiceCallSpecialProjectUpdated>(x =>
                {
                    x.ServiceCallNumber = serviceCall.ServiceCallNumber;
                    x.SpecialProject = serviceCall.IsSpecialProject;
                    x.SpecialProjectReason = message.SpecialProjectReason;
                    x.SpecialProjectDate = message.SpecialProjectDate;
                });
            }
        }
    }
}