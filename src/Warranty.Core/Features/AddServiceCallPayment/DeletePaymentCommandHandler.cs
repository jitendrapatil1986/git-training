namespace Warranty.Core.Features.AddServiceCallPayment
{
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using InnerMessages;
    using NPoco;
    using NServiceBus;
    using Security;

    public class DeletePaymentCommandHandler : ICommandHandler<DeletePaymentCommand>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _activityLogger;
        private readonly IBus _bus;
        private readonly IUserSession _userSession;

        public DeletePaymentCommandHandler(IDatabase database, IActivityLogger activityLogger, IBus bus, IUserSession userSession)
        {
            _database = database;
            _activityLogger = activityLogger;
            _bus = bus;
            _userSession = userSession;
        }

        public void Handle(DeletePaymentCommand message)
        {
            using (_database)
            {
                var payment = _database.SingleById<Payment>(message.PaymentId);
                var backcharge = _database.SingleOrDefault<Backcharge>("Where PaymentId = @0", message.PaymentId);
                
                if (backcharge != null)
                {
                    backcharge.BackchargeStatus = BackchargeStatus.RequestedDelete;
                    _database.Update(backcharge);
                }

                payment.PaymentStatus = PaymentStatus.RequestedDelete;
                _database.Update(payment);

                _activityLogger.Write("Payment delete requested", string.Empty, payment.PaymentId, ActivityType.PaymentDelete, ReferenceType.LineItem);

                _bus.Send<NotifyPaymentDeleted>(x =>
                {
                    x.PaymentId = payment.PaymentId;
                    x.UserName = _userSession.GetCurrentUser().LoginName;
                });
            }
        }
    }
}