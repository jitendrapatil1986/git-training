namespace Warranty.Server.Handlers.Payments
{
    using System;
    using Configuration;
    using Core.Entities;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyBackchargeApprovedHandler : IHandleMessages<NotifyBackchargeApproved>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;

        public NotifyBackchargeApprovedHandler(IBus bus, IDatabase database)
        {
            _bus = bus;
            _database = database;
        }

        public void Handle(NotifyBackchargeApproved message)
        {
            using (_database)
            {
                var backcharge = _database.SingleById<Backcharge>(message.BackchargeId);
                var payment = _database.SingleById<Payment>(backcharge.PaymentId);

                var command = new Accounting.Commands.Backcharges.RequestBackchargeApproval()
                {
                    PaymentJdeIdentifier = payment.JdeIdentifier,
                    BackchargeJdeIdentifier = backcharge.JdeIdentifier,
                    BackchargeId = backcharge.BackchargeId.ToString(),
                    ProgramId = WarrantyConstants.ProgramId,
                    DateApproved = DateTime.Today,
                    ApprovedBy = message.UserName,
                };
                _bus.Send(command);
            }
        
        }
    }
}
