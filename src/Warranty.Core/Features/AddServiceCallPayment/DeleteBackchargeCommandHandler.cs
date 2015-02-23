namespace Warranty.Core.Features.AddServiceCallPayment
{
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class DeleteBackchargeCommandHandler : ICommandHandler<DeleteBackchargeCommand>
    {
        private readonly IDatabase _database;
        private readonly IBus _bus;
        private readonly IActivityLogger _activityLogger;

        public DeleteBackchargeCommandHandler(IDatabase database, IBus bus, IActivityLogger activityLogger)
        {
            _database = database;
            _bus = bus;
            _activityLogger = activityLogger;
        }

        public void Handle(DeleteBackchargeCommand message)
        {
            using (_database)
            {
                var backcharge = _database.SingleById<Backcharge>(message.BackchargeId);

                backcharge.BackchargeStatus = BackchargeStatus.RequestedDelete;
                _database.Update(backcharge);
                
                _activityLogger.Write("Stand alone backcharge delete requested", string.Empty, backcharge.BackchargeId, ActivityType.BackchargeDelete, ReferenceType.LineItem);

                _bus.Send<NotifyBackchargeDeleted>(x =>
                {
                    x.BackchargeId = backcharge.BackchargeId;
                });
            }
        }
    }
}