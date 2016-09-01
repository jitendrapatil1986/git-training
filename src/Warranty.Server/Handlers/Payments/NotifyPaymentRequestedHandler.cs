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
                        VarianceCode = message.HasAssociatedBackcharge ? Configuration.WarrantyConstants.VarianceCodeForBackcharge : Configuration.WarrantyConstants.VarianceCode,
                        PaymentIdentifier = payment.PaymentId.ToString(),
                        ProgramId = Configuration.WarrantyConstants.ProgramId,
                        ObjectAccount = payment.ObjectAccount,
                        VarianceExplanation = ToPaymentComment(payment.Comments),
                        SendCheckToPC = payment.SendCheckToPC
                    };

                _bus.Send(paymentRequest);
            }
        }

        private string ToPaymentComment(string comment)
        {
            if (comment == null)
                return null;

            if (string.IsNullOrWhiteSpace(comment) || comment.Length <= 240)
                return comment.Trim(' ');

            return comment.Substring(0, 240);
        }
    }
}