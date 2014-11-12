namespace Warranty.Server.Handlers.Payments
{
    using Accounting.Commands.Payments;
    using Core.Entities;
    using Core.Security;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyPaymentRequestedHandler : IHandleMessages<NotifyPaymentRequested>
    {
        private readonly IDatabase _database;
        private readonly IBus _bus;
        private readonly IUserSession _userSession;

        public NotifyPaymentRequestedHandler(IDatabase database, IBus bus, IUserSession userSession)
        {
            _database = database;
            _bus = bus;
            _userSession = userSession;
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
                        CommunityNumber = payment.JobNumber,  //accounting pulls the substring
                        InvoiceDate = payment.CreatedDate,
                        InvoiceNumber = payment.InvoiceNumber,
                        Username = _userSession.GetCurrentUser().LoginName,
                        VendorNumber = payment.VendorNumber,
                        CostCode = payment.CostCode,
                        PaymentType = Configuration.WarrantyConstants.PaymentType,
                        VarianceCode = Configuration.WarrantyConstants.VarianceCode,
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