namespace Warranty.Core.Features.DeleteServiceCall
{
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class AdminDeleteServiceCallCommandHandler : ICommandHandler<AdminDeleteServiceCallCommand>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _activityLogger;
        private readonly IBus _bus;

        public AdminDeleteServiceCallCommandHandler(IDatabase database, IActivityLogger activityLogger, IBus bus)
        {
            _database = database;
            _activityLogger = activityLogger;
            _bus = bus;
        }

        public void Handle(AdminDeleteServiceCallCommand message)
        {
            using (_database.Transaction)
            {
                _database.BeginTransaction();

                _database.DeleteWhere<ServiceCallLineItem>("ServiceCallId = ?", message.ServiceCallId);
                _database.DeleteWhere<ServiceCallNote>("ServiceCallId = ?", message.ServiceCallId);
                _database.DeleteWhere<ServiceCallAttachment>("ServiceCallId = ?", message.ServiceCallId);
                _database.DeleteWhere<ServiceCall>("ServiceCallId = ?", message.ServiceCallId);

                _bus.Send(new NotifyServiceCallDeleted
                {
                    ServiceCallId = message.ServiceCallId
                });

                _database.CompleteTransaction();

                _activityLogger.Write("Service Call deleted", string.Empty, message.ServiceCallId, ActivityType.ServiceCallDelete, ReferenceType.ServiceCall);
            }
        }
    }
}