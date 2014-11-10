namespace Warranty.Core.Features.AddServiceCallPayment
{
    using System;
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class AddPaymentOnHoldCommandHandler : ICommandHandler<AddPaymentOnHoldCommand, AddPaymentOnHoldCommandHandlerResponse>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _activityLogger;
        private readonly IBus _bus;

        public AddPaymentOnHoldCommandHandler(IDatabase database, IActivityLogger activityLogger, IBus bus)
        {
            _database = database;
            _activityLogger = activityLogger;
            _bus = bus;
        }

        public AddPaymentOnHoldCommandHandlerResponse Handle(AddPaymentOnHoldCommand message)
        {
            using (_database)
            {
                var payment = _database.SingleById<Payment>(message.PaymentId);

                var newStatus = PaymentStatus.RequestedHold;
                payment.PaymentStatus = newStatus;
                payment.HoldComments = message.Message;
                payment.HoldDate = DateTime.UtcNow;

                _database.Update(payment);

                _activityLogger.Write("Payment was requested to be put on hold", message.Message, payment.PaymentId, ActivityType.PaymentOnHold, ReferenceType.Homeowner);

                _bus.Send<NotifyPaymentOnHold>(x => x.PaymentId = payment.PaymentId);

                return new AddPaymentOnHoldCommandHandlerResponse
                        {
                            NewStatusDisplayName = newStatus.DisplayName,
                            Date = payment.HoldDate.Value
                        };
            }
        }
    }

    public class AddPaymentOnHoldCommandHandlerResponse
    {
        public string NewStatusDisplayName { get; set; }
        public DateTime Date { get; set; }
    }

}