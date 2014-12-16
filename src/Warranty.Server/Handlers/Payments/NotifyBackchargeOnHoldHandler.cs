namespace Warranty.Server.Handlers.Payments
{
    using System;
    using Configuration;
    using Core.Entities;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyBackchargeOnHoldHandler : IHandleMessages<NotifyBackchargeOnHold>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;

        public NotifyBackchargeOnHoldHandler(IBus bus, IDatabase database)
        {
            _bus = bus;
            _database = database;
        }

        public void Handle(NotifyBackchargeOnHold message)
        {
            using (_database)
            {
                var backcharge = _database.SingleById<Backcharge>(message.BackchargeId);
                var payment = _database.SingleById<Payment>(backcharge.PaymentId);

                var command = new Accounting.Commands.Backcharges.RequestBackchargeHold()
                {
                    BackchargeJdeIdentifier = backcharge.JdeIdentifier,
                    PaymentJdeIdentifier = payment.JdeIdentifier,
                    BackchargeId = backcharge.BackchargeId.ToString(),
                    ProgramId = WarrantyConstants.ProgramId,
                    DateOnHold = DateTime.Today,
                    OnHoldBy = message.UserName,
                    OnHoldReason = backcharge.HoldComments
                };
                _bus.Send(command);
            }
        }
    }
}
