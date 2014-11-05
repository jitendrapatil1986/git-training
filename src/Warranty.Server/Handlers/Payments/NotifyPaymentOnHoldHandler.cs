namespace Warranty.Server.Handlers.Payments
{
    using System;
    using Configuration;
    using Core.Entities;
    using Core.Security;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyPaymentOnHoldHandler : IHandleMessages<NotifyPaymentOnHold>
    {

        private readonly IBus _bus;
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public NotifyPaymentOnHoldHandler(IBus bus, IDatabase database, IUserSession userSession)
        {
            _bus = bus;
            _database = database;
            _userSession = userSession;
        }

        public void Handle(NotifyPaymentOnHold message)
        {
            using (_database)
            {
                var payment = _database.SingleById<Payment>(message.PaymentId);

                var command = new Accounting.Commands.Payments.RequestPaymentHold()
                {
                    PaymentJdeIdentifier = payment.JdeIdentifier,
                    ProgramId = WarrantyConstants.WARRANTY,
                    DateOnHold = DateTime.Today,
                    OnHoldBy = _userSession.GetCurrentUser().LoginName,
                    OnHoldReason = payment.HoldComments
                };
                _bus.Send(command);
            }   
        }
    }
}
