namespace Warranty.Core.Features.DeleteServiceCall
{
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class DeleteServiceCallCommandHandler : ICommandHandler<DeleteServiceCallCommand>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _activityLogger;
        private readonly IBus _bus;

        public DeleteServiceCallCommandHandler(IDatabase database, IActivityLogger activityLogger, IBus bus)
        {
            _database = database;
            _activityLogger = activityLogger;
            _bus = bus;
        }

        public void Handle(DeleteServiceCallCommand message)
        {
            using (_database)
            {
                var serviceCall = _database.SingleById<ServiceCall>(message.ServiceCallId);
                if (serviceCall.ServiceCallStatus == ServiceCallStatus.Requested)
                {
                    _database.Delete(serviceCall);
                    _bus.Send(new NotifyServiceCallDeleted
                        {
                            ServiceCallId = serviceCall.ServiceCallId
                        });
                    _activityLogger.Write("Service Call deleted", string.Empty, serviceCall.ServiceCallId,
                                          ActivityType.ServiceCallDelete, ReferenceType.ServiceCall);
                }
            }
        }
    }
}