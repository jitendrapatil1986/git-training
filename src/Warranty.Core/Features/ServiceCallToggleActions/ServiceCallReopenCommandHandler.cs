namespace Warranty.Core.Features.ServiceCallToggleActions
{
    using System;
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class ServiceCallReopenCommandHandler : ICommandHandler<ServiceCallReopenCommand>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _logger;
        private readonly IBus _bus;

        public ServiceCallReopenCommandHandler(IDatabase database, IActivityLogger logger, IBus bus)
        {
            if (bus == null) throw new ArgumentNullException("bus");
            _database = database;
            _logger = logger;
            _bus = bus;
        }

        public void Handle(ServiceCallReopenCommand message)
        {
            using (_database)
            {
                var serviceCall = _database.SingleOrDefaultById<ServiceCall>(message.ServiceCallId);
                serviceCall.ServiceCallStatus = ServiceCallStatus.Open;
                serviceCall.CompletionDate = null;
                _database.Update(serviceCall);

                _bus.Send(new NotifyServiceCallStatusChanged
                    {
                        ServiceCallId = serviceCall.ServiceCallId
                    });

                const string activityName = "Service call was Reopened.";

                _logger.Write(activityName, message.Text, message.ServiceCallId, ActivityType.Reopen, ReferenceType.ServiceCall);
            }
        }
    }
}