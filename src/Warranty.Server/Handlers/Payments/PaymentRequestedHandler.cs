namespace Warranty.Server.Handlers.Payments
{
    using Accounting.Commands.Payments;
    using Core.Entities;
    using Core.Security;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class PaymentRequestedHandler : IHandleMessages<NotifyRequestedPayment>
    {
        private readonly IDatabase _database;
        private readonly IBus _bus;
        private readonly IUserSession _userSession;

        public PaymentRequestedHandler(IDatabase database, IBus bus, IUserSession userSession)
        {
            _database = database;
            _bus = bus;
            _userSession = userSession;
        }

        public void Handle(NotifyRequestedPayment message)
        {
            using (_database)
            {
                var payment = _database.SingleById<Payment>(message.PaymentId);
                var paymentRequest = new RequestExtraWorkOrderPayment
                    {
                        JobNumber = payment.JobNumber,
                        Amount = payment.Amount,
                        CommunityNumber = string.IsNullOrEmpty(payment.JobNumber) ? "" : payment.JobNumber.Substring(0, 4),  //get first 4 chs. bc it is always the community number for the job.
                        InvoiceDate = payment.CreatedDate,
                        InvoiceNumber = payment.InvoiceNumber,
                        Username = _userSession.GetCurrentUser().LoginName,
                        VendorNumber = payment.VendorNumber,
                    };

                _bus.Send(paymentRequest);
            }
        }
    }
}