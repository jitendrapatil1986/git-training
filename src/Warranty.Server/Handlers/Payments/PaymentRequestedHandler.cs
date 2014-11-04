namespace Warranty.Server.Handlers.Payments
{
    using Accounting.Commands.Payments;
    using Core.Entities;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class PaymentRequestedHandler : IHandleMessages<NotifyRequestedPayment>
    {
        private readonly IDatabase _database;
        private readonly IBus _bus;

        public PaymentRequestedHandler(IDatabase database, IBus bus)
        {
            _database = database;
            _bus = bus;
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
                        Username = payment.CreatedBy,
                        VendorNumber = payment.VendorNumber,
                    };

                _bus.Send(paymentRequest);
            }
        }
    }
}