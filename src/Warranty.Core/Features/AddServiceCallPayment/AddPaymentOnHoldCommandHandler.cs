namespace Warranty.Core.Features.AddServiceCallPayment
{
    using Entities;
    using Enumerations;
    using NPoco;

    public class AddPaymentOnHoldCommandHandler : ICommandHandler<AddPaymentOnHoldCommand>
    {
        private readonly IDatabase _database;

        public AddPaymentOnHoldCommandHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(AddPaymentOnHoldCommand message)
        {
            using (_database)
            {
                var payment = _database.SingleById<Payment>(message.PaymentId);
                payment.PaymentStatus = PaymentStatus.Hold;
                _database.Update(payment);
            }
        }
    }
}