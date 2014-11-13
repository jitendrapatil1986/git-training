namespace Warranty.Core.Features.AddServiceCallPayment
{
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using InnerMessages;
    using NPoco;
    using NServiceBus;
    using Security;

    public class ApproveBackchargeCommandHandler : ICommandHandler<ApproveBackchargeCommand, string>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _activityLogger;
        private readonly IBus _bus;
        private readonly IUserSession _userSession;

        public ApproveBackchargeCommandHandler(IDatabase database, IActivityLogger activityLogger, IBus bus, IUserSession userSession)
        {
            _database = database;
            _activityLogger = activityLogger;
            _bus = bus;
            _userSession = userSession;
        }

        public string Handle(ApproveBackchargeCommand message)
        {
            using (_database)
            {
                var backcharge = _database.SingleById<Backcharge>(message.BackchargeId);

                var newStatus = BackchargeStatus.RequestedApproval;
                backcharge.BackchargeStatus = newStatus;

                _database.Update(backcharge);

                _activityLogger.Write("Backcharge approval requested", string.Empty, backcharge.PaymentId, ActivityType.BackchargeApprove, ReferenceType.LineItem);

                _bus.Send<NotifyBackchargeApproved>(x =>
                    {
                        x.BackchargeId = backcharge.BackchargeId;
                        x.UserName = _userSession.GetCurrentUser().LoginName;
                    });

                return backcharge.BackchargeStatus.DisplayName;
            }
        }
    }
}