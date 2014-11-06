namespace Warranty.Core.Features.AddServiceCallPayment
{
    using Entities;
    using NPoco;

    public class DeleteServiceCallLineItemPaymentCommandHandler : ICommandHandler<DeleteServiceCallLineItemPaymentCommand>
    {
        private readonly IDatabase _database;

        public DeleteServiceCallLineItemPaymentCommandHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(DeleteServiceCallLineItemPaymentCommand message)
        {
            using (_database)
            {
                var payment = _database.SingleById<Payment>(message.PaymentId);
                var backcharge = _database.SingleOrDefault<Backcharge>("Where PaymentId = @0", message.PaymentId);
                if (backcharge != null)
                {
                    _database.Delete(backcharge);
                }
                _database.Delete(payment);
            }
        }
    }
}