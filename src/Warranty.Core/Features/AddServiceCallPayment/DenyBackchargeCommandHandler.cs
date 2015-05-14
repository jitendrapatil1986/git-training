namespace Warranty.Core.Features.AddServiceCallPayment
{
    using System;
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using InnerMessages;
    using NPoco;
    using NServiceBus;
    using Common.Security.User.Session;

    public class DenyBackchargeCommandHandler : ICommandHandler<DenyBackchargeCommand, DenyBackchargeCommandHandler.DenyBackchargeCommandHandlerResponse>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _activityLogger;
        private readonly IBus _bus;
        private readonly IUserSession _userSession;

        public DenyBackchargeCommandHandler(IDatabase database, IActivityLogger activityLogger, IBus bus, IUserSession userSession)
        {
            _database = database;
            _activityLogger = activityLogger;
            _bus = bus;
            _userSession = userSession;
        }

        public DenyBackchargeCommandHandlerResponse Handle(DenyBackchargeCommand message)
        {
            using (_database)
            {
                var backcharge = _database.SingleById<Backcharge>(message.BackchargeId);

                var newStatus = BackchargeStatus.RequestedDeny;
                backcharge.BackchargeStatus = newStatus;
                backcharge.DenyComments = message.Message;
                backcharge.DenyDate = DateTime.UtcNow;

                _database.Update(backcharge);

                _activityLogger.Write("Backcharge deny requested", message.Message, backcharge.BackchargeId, ActivityType.BackchargeDeny, ReferenceType.LineItem);

                _bus.Send<NotifyBackchargeDeny>(x =>
                    {
                        x.BackchargeId = backcharge.BackchargeId;
                        x.UserName = _userSession.GetCurrentUser().LoginName;
                    });

                return new DenyBackchargeCommandHandlerResponse
                    {
                        NewStatusDisplayName = newStatus.DisplayName,
                        Date = backcharge.DenyDate.Value
                    };
            }
        }

        public class DenyBackchargeCommandHandlerResponse
        {
            public string NewStatusDisplayName { get; set; }
            public DateTime Date { get; set; }
        }
    }
}