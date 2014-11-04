namespace Warranty.Server.Handlers.Payments
{
    using System;
    using Accounting.Commands.Backcharges;
    using Core.Entities;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class BackchargeRequestedHandler : IHandleMessages<NotifyRequestedBackcharge>
    {
        private readonly IDatabase _database;
        private readonly IBus _bus;

        public BackchargeRequestedHandler(IBus bus, IDatabase database)
        {
            _bus = bus;
            _database = database;
        }

        public void Handle(NotifyRequestedBackcharge message)
        {
            using (_database)
            {
                var backcharge = _database.SingleById<Backcharge>(message.BackchargeId);
                var payment = _database.SingleById<Payment>(backcharge.PaymentId);

                var backchargeRequest = new RequestBackcharge
                {
                    JobNumber = payment.JobNumber,
                    BackchargeAmount = backcharge.BackchargeAmount,
                    VendorNumber = backcharge.BackchargeVendorNumber,
                    ResponseFromVendor = backcharge.BackchargeResponseFromVendor,
                    ReasonForBackcharge = backcharge.BackchargeReason,
                    PersonNotified = backcharge.PersonNotified,
                    DateNotified = backcharge.PersonNotifiedDate,
                    PhoneNumber = backcharge.PersonNotifiedPhoneNumber,
                    PaymentAmount = payment.Amount,
                    PaymentInvoiceNumber = payment.InvoiceNumber,
                    Username = backcharge.CreatedBy,
                    PaymentVendorNumber = payment.VendorNumber,
                };

                _bus.Send(backchargeRequest);
            }
        }
    }
}