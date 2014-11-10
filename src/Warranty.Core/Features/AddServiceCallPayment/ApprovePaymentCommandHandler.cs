namespace Warranty.Core.Features.AddServiceCallPayment
{
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class ApprovePaymentCommandHandler : ICommandHandler<ApprovePaymentCommand, string>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _activityLogger;
        private readonly IBus _bus;

        public ApprovePaymentCommandHandler(IDatabase database, IActivityLogger activityLogger, IBus bus)
        {
            _database = database;
            _activityLogger = activityLogger;
            _bus = bus;
        }

        public string Handle(ApprovePaymentCommand message)
        {
            using (_database)
            {
                var payment = _database.SingleById<Payment>(message.PaymentId);

                var newStatus = PaymentStatus.RequestedApproval;
                payment.PaymentStatus = newStatus;

                _database.Update(payment);

                _activityLogger.Write("Payment approval requested", string.Empty, payment.PaymentId, ActivityType.PaymentOnHold, ReferenceType.LineItem);

                _bus.Send<NotifyPaymentApproved>(x => x.PaymentId = payment.PaymentId);

                return payment.PaymentStatus.DisplayName;
            }
        }
    }
}