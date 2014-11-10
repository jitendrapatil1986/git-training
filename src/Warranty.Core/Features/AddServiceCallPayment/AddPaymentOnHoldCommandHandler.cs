namespace Warranty.Core.Features.AddServiceCallPayment
{
    using System;
    using ActivityLogger;
    using Entities;
    using Enumerations;
    using NPoco;

    public class AddPaymentOnHoldCommandHandler : ICommandHandler<AddPaymentOnHoldCommand, AddPaymentOnHoldCommandHandlerResponse>
    {
        private readonly IDatabase _database;
        private readonly IActivityLogger _activityLogger;

        public AddPaymentOnHoldCommandHandler(IDatabase database, IActivityLogger activityLogger)
        {
            _database = database;
            _activityLogger = activityLogger;
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