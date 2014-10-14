namespace Warranty.Core.Features.ServiceCallToggleActions
{
    using System;
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class ServiceCallCompleteCommandHandler : ICommandHandler<ServiceCallCompleteCommand>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _logger;
        private readonly IBus _bus;

        public ServiceCallCompleteCommandHandler(IDatabase database, IActivityLogger logger, IBus bus)
        {
            _database = database;
            _logger = logger;
            _bus = bus;
        }

        public void Handle(ServiceCallCompleteCommand message)
        {
            using (_database)
            {
                var serviceCall = _database.SingleOrDefaultById<ServiceCall>(message.ServiceCallId);
                serviceCall.ServiceCallStatus = ServiceCallStatus.Complete;
                serviceCall.CompletionDate = DateTime.UtcNow;
                _database.Update(serviceCall);
                
                _bus.Send<NotifyServiceCallCompleted>(x =>
                {
                    x.ServiceCallId = serviceCall.ServiceCallId;
                });

                const string activityName = "Service call was completed.";

                _logger.Write(activityName, null, message.ServiceCallId, ActivityType.Complete, ReferenceType.ServiceCall);
            }
        }
    }
}