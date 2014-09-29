namespace Warranty.Core.Features.ServiceCallToggleActions
{
    using System;
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using NPoco;

    public class ServiceCallCloseCommandHandler : ICommandHandler<ServiceCallCloseCommand>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _logger;

        public ServiceCallCloseCommandHandler(IDatabase database, IActivityLogger logger)
        {
            _database = database;
            _logger = logger;
        }

        public void Handle(ServiceCallCloseCommand message)
        {
            using (_database)
            {
                var serviceCall = _database.SingleOrDefaultById<ServiceCall>(message.ServiceCallId);
                serviceCall.ServiceCallStatus = ServiceCallStatus.Closed;
                serviceCall.ClosedDate = DateTime.UtcNow;
                _database.Update(serviceCall);

                const string activityName = "Service call was closed.";

                _logger.Write(activityName, null, message.ServiceCallId, ActivityType.Close, ReferenceType.ServiceCall);
            }
        }
    }
}