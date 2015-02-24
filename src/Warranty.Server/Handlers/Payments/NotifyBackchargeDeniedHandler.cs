namespace Warranty.Server.Handlers.Payments
{
    using System;
    using Configuration;
    using Core.Entities;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyBackchargeDeniedHandler : IHandleMessages<NotifyBackchargeDeny>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;

        public NotifyBackchargeDeniedHandler(IBus bus, IDatabase database)
        {
            _bus = bus;
            _database = database;
        }

        public void Handle(NotifyBackchargeDeny message)
        {
            using (_database)
            {
                var backcharge = _database.SingleById<Backcharge>(message.BackchargeId);
                var payment = _database.SingleOrDefaultById<Payment>(backcharge.PaymentId);

                var command = new Accounting.Commands.Backcharges.RequestBackchargeDenial()
                {
                    BackchargeJdeIdentifier = backcharge.JdeIdentifier,
                    PaymentJdeIdentifier = payment == null ? null : payment.JdeIdentifier,
                    BackchargeId = backcharge.BackchargeId.ToString(),
                    ProgramId = WarrantyConstants.ProgramId,
                    DateDenied = DateTime.Today,
                    DeniedBy = message.UserName,
                    DeniedReason = backcharge.DenyComments
                };
                _bus.Send(command);
            }
        }
    }
}
