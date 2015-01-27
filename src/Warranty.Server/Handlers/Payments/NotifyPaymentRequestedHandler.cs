namespace Warranty.Server.Handlers.Payments
{
    using Accounting.Commands.Payments;
    using Core.Entities;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyPaymentRequestedHandler : IHandleMessages<NotifyPaymentRequested>
    {
        private readonly IDatabase _database;
        private readonly IBus _bus;

        public NotifyPaymentRequestedHandler(IDatabase database, IBus bus)
        {
            _database = database;
            _bus = bus;
        }

        public void Handle(NotifyPaymentRequested message)
        {
            using (_database)
            {
                
                var payment = _database.SingleById<Payment>(message.PaymentId);
                var backcharge = _database.SingleOrDefault<Backcharge>("WHERE PaymentId = @0", message.PaymentId);

                var paymentRequest = new RequestExtraWorkOrderPayment
                    {
                        JobNumber = payment.JobNumber,
                        Amount = payment.Amount,
                        CommunityNumber = payment.CommunityNumber,  //accounting pulls the substring
                        InvoiceDate = payment.CreatedDate,
                        InvoiceNumber = payment.InvoiceNumber,
                        Username = message.Username,
                        VendorNumber = payment.VendorNumber,
                        CostCode = payment.CostCode,
                        PaymentType = Configuration.WarrantyConstants.PaymentType,
                        VarianceCode = backcharge == null ? Configuration.WarrantyConstants.VarianceCode : Configuration.WarrantyConstants.VarianceCodeForBackcharge,
                        PaymentIdentifier = payment.PaymentId.ToString(),
                        ProgramId = Configuration.WarrantyConstants.ProgramId,
                        ObjectAccount = payment.ObjectAccount,
                        VarianceExplanation = ""
                    };

                _bus.Send(paymentRequest);
            }
        }
    }
}