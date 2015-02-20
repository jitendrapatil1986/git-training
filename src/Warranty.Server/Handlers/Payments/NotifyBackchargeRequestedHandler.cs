namespace Warranty.Server.Handlers.Payments
{
    using Accounting.Commands.Backcharges;
    using Configuration;
    using Core.Entities;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyBackchargeRequestedHandler : IHandleMessages<NotifyBackchargeRequested>
    {
        private readonly IDatabase _database;
        private readonly IBus _bus;

        public NotifyBackchargeRequestedHandler(IBus bus, IDatabase database)
        {
            _bus = bus;
            _database = database;
        }

        public void Handle(NotifyBackchargeRequested message)
        {
            using (_database)
            {
                var backcharge = _database.SingleById<Backcharge>(message.BackchargeId);
                var payment = _database.SingleOrDefaultById<Payment>(backcharge.PaymentId);

                var backchargeRequest = new RequestBackcharge
                {
                    JobNumber = payment == null ? backcharge.JobNumber.Substring(0, 4) : payment.JobNumber.Substring(0,4),
                    BackchargeAmount = backcharge.BackchargeAmount,
                    VendorNumber = backcharge.BackchargeVendorNumber,
                    ResponseFromVendor = backcharge.BackchargeResponseFromVendor,
                    ReasonForBackcharge = backcharge.BackchargeReason,
                    PersonNotified = backcharge.PersonNotified,
                    DateNotified = backcharge.PersonNotifiedDate,
                    PhoneNumber = backcharge.PersonNotifiedPhoneNumber,
                    PaymentAmount = payment == null ? 0 : payment.Amount,
                    PaymentInvoiceNumber = payment == null ? "" : payment.InvoiceNumber,
                    Username = message.Username,
                    PaymentVendorNumber = payment == null ? null : payment.VendorNumber,
                    CostCode = backcharge.CostCode,
                    ProgramId = WarrantyConstants.ProgramId,
                    ObjectAccount = payment == null ? backcharge.ObjectAccount : payment.ObjectAccount,
                    BuilderNumber = message.EmployeeNumber,
                    OptionNumber = payment == null ? backcharge.JobNumber : payment.JobNumber,
                    BackchargeIdentifier = backcharge.BackchargeId.ToString()
                };

                _bus.Send(backchargeRequest);
            }
        }
    }
}