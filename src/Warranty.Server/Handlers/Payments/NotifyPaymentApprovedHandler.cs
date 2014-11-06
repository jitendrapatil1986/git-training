namespace Warranty.Server.Handlers.Payments
{
    using System;
    using Configuration;
    using Core.Entities;
    using Core.Security;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyPaymentApprovedHandler : IHandleMessages<NotifyPaymentApproved>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public NotifyPaymentApprovedHandler(IBus bus, IDatabase database, IUserSession userSession)
        {
            _bus = bus;
            _database = database;
            _userSession = userSession;
        }

        public void Handle(NotifyPaymentApproved message)
        {
            using (_database)
            {
                var payment = _database.SingleById<Payment>(message.PaymentId);

                var command = new Accounting.Commands.Payments.RequestPaymentApproval()
                {
                    PaymentJdeIdentifier = payment.JdeIdentifier,
                    PaymentId = payment.PaymentId.ToString(),
                    ProgramId = WarrantyConstants.ProgramId,
                    DateApproved = DateTime.Today,
                    ApprovedBy = _userSession.GetCurrentUser().LoginName
                };
                _bus.Send(command);
            }
        }
    }
}
