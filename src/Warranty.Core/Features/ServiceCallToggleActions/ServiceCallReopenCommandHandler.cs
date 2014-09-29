namespace Warranty.Core.Features.ServiceCallToggleActions
{
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using NPoco;

    public class ServiceCallReopenCommandHandler : ICommandHandler<ServiceCallReopenCommand>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _logger;

        public ServiceCallReopenCommandHandler(IDatabase database, IActivityLogger logger)
        {
            _database = database;
            _logger = logger;
        }

        public void Handle(ServiceCallReopenCommand message)
        {
            using (_database)
            {
                var serviceCall = _database.SingleOrDefaultById<ServiceCall>(message.ServiceCallId);
                serviceCall.ServiceCallStatus = ServiceCallStatus.Open;
                _database.Update(serviceCall);

                const string activityName = "Service call was Reopend.";

                _logger.Write(activityName, message.Text, message.ServiceCallId, ActivityType.Reopen, ReferenceType.ServiceCall);
            }
        }
    }
}