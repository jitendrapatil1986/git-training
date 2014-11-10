namespace Warranty.Server.Handlers.Payments
{
    using Accounting.Commands.Backcharges;
    using Core.Entities;
    using Core.Security;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyRequestedBackchargeHandler : IHandleMessages<NotifyRequestedBackcharge>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;
        private readonly IBus _bus;

        public NotifyRequestedBackchargeHandler(IBus bus, IDatabase database, IUserSession userSession)
        {
            _bus = bus;
            _database = database;
            _userSession = userSession;
        }

        public void Handle(NotifyRequestedBackcharge message)
        {
            using (_database)
            {
                var backcharge = _database.SingleById<Backcharge>(message.BackchargeId);
                var payment = _database.SingleById<Payment>(backcharge.PaymentId);
                var job = _database.Single<Job>("SELECT * FROM Job WHERE JobNumber = @0", payment.JobNumber);

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
                    PaymentAmount = backcharge.BackchargeAmount,
                    PaymentInvoiceNumber = payment.InvoiceNumber,
                    Username = _userSession.GetCurrentUser().LoginName,
                    PaymentVendorNumber = payment.VendorNumber,
                    CostCode = backcharge.CostCode,
                    ProgramId = Configuration.WarrantyConstants.ProgramId,
                    ObjectAccount = job.IsOlderThanTwoYears ? Configuration.WarrantyConstants.OverTwoYearLaborCode : Configuration.WarrantyConstants.UnderTwoYearLaborCode,
                    BuilderNumber = _userSession.GetCurrentUser().EmployeeNumber,
                    BackchargeIdentifier = backcharge.BackchargeId.ToString()
                };

                _bus.Send(backchargeRequest);
            }
        }
    }
}