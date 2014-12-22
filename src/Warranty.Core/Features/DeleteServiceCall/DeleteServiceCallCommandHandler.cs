namespace Warranty.Core.Features.DeleteServiceCall
{
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using NPoco;

    public class DeleteServiceCallCommandHandler : ICommandHandler<DeleteServiceCallCommand>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _activityLogger;

        public DeleteServiceCallCommandHandler(IDatabase database, IActivityLogger activityLogger)
        {
            _database = database;
            _activityLogger = activityLogger;
        }

        public void Handle(DeleteServiceCallCommand message)
        {
            using (_database)
            {
                var serviceCall = _database.SingleById<ServiceCall>(message.ServiceCallId);
                if (serviceCall.ServiceCallStatus == ServiceCallStatus.Requested)
                {
                    _database.Delete(serviceCall);
                    _activityLogger.Write("Service Call deleted", string.Empty, serviceCall.ServiceCallId,
                                          ActivityType.ServiceCallDelete, ReferenceType.ServiceCall);
                }
            }
        }
    }
}