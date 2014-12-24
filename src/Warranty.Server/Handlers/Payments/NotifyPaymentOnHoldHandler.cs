namespace Warranty.Server.Handlers.Payments
{
    using System;
    using Configuration;
    using Core.Entities;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyPaymentOnHoldHandler : IHandleMessages<NotifyPaymentOnHold>
    {

        private readonly IBus _bus;
        private readonly IDatabase _database;

        public NotifyPaymentOnHoldHandler(IBus bus, IDatabase database)
        {
            _bus = bus;
            _database = database;
        }

        public void Handle(NotifyPaymentOnHold message)
        {
            using (_database)
            {
                var payment = _database.SingleById<Payment>(message.PaymentId);

                var command = new Accounting.Commands.Payments.RequestPaymentHold()
                {
                    PaymentJdeIdentifier = payment.JdeIdentifier,
                    PaymentId = payment.PaymentId.ToString(),
                    ProgramId = WarrantyConstants.ProgramId,
                    DateOnHold = DateTime.Today,
                    OnHoldBy = message.Username,
                    OnHoldReason = payment.HoldComments
                };
                _bus.Send(command);
            }   
        }
    }
}
