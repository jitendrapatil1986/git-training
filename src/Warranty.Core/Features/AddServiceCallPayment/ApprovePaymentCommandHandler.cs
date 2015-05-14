namespace Warranty.Core.Features.AddServiceCallPayment
{
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using InnerMessages;
    using NPoco;
    using NServiceBus;
    using Common.Security.User.Session;

    public class ApprovePaymentCommandHandler : ICommandHandler<ApprovePaymentCommand, string>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _activityLogger;
        private readonly IBus _bus;
        private readonly IUserSession _userSession;

        public ApprovePaymentCommandHandler(IDatabase database, IActivityLogger activityLogger, IBus bus, IUserSession userSession)
        {
            _database = database;
            _activityLogger = activityLogger;
            _bus = bus;
            _userSession = userSession;
        }

        public string Handle(ApprovePaymentCommand message)
        {
            using (_database)
            {
                var payment = _database.SingleById<Payment>(message.PaymentId);

                var newStatus = PaymentStatus.RequestedApproval;
                payment.PaymentStatus = newStatus;

                _database.Update(payment);

                _activityLogger.Write("Payment approval requested", string.Empty, payment.PaymentId, ActivityType.PaymentApprove, ReferenceType.LineItem);

                _bus.Send<NotifyPaymentApproved>(x =>
                {
                    x.PaymentId = payment.PaymentId;
                    x.UserName = _userSession.GetCurrentUser().LoginName;
                });

                return payment.PaymentStatus.DisplayName;
            }
        }
    }
}