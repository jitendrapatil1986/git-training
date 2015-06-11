namespace Warranty.Core.Features.AddServiceCallPayment
{
    using System;
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using InnerMessages;
    using NPoco;
    using NServiceBus;
    using Common.Security.Session;

    public class HoldBackchargeCommandHandler : ICommandHandler<HoldBackchargeCommand, HoldBackchargeCommandHandler.HoldBackchargeCommandHandlerResponse>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _activityLogger;
        private readonly IBus _bus;
        private readonly IUserSession _userSession;

        public HoldBackchargeCommandHandler(IDatabase database, IActivityLogger activityLogger, IBus bus, IUserSession userSession)
        {
            _database = database;
            _activityLogger = activityLogger;
            _bus = bus;
            _userSession = userSession;
        }

        public HoldBackchargeCommandHandlerResponse Handle(HoldBackchargeCommand message)
        {
            using (_database)
            {
                var backcharge = _database.SingleById<Backcharge>(message.BackchargeId);

                var newStatus = BackchargeStatus.RequestedHold;
                backcharge.BackchargeStatus = newStatus;
                backcharge.HoldComments = message.Message;
                backcharge.HoldDate = DateTime.UtcNow;

                _database.Update(backcharge);

                _activityLogger.Write("Backcharge hold requested", message.Message, backcharge.BackchargeId, ActivityType.BackchargeOnHold, ReferenceType.LineItem);

                _bus.Send<NotifyBackchargeOnHold>(x =>
                {
                    x.BackchargeId = backcharge.BackchargeId;
                    x.UserName = _userSession.GetCurrentUser().LoginName;
                });

                return new HoldBackchargeCommandHandlerResponse
                {
                    NewStatusDisplayName = newStatus.DisplayName,
                    Date = backcharge.HoldDate.Value
                };
            }
        }

        public class HoldBackchargeCommandHandlerResponse
        {
            public string NewStatusDisplayName { get; set; }
            public DateTime Date { get; set; }
        }
    }
}