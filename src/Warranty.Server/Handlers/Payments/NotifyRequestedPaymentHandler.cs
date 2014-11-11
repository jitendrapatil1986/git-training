namespace Warranty.Server.Handlers.Payments
{
    using Accounting.Commands.Payments;
    using Core.Entities;
    using Core.Security;
    using InnerMessages;
    using NPoco;
    using NServiceBus;

    public class NotifyRequestedPaymentHandler : IHandleMessages<NotifyRequestedPayment>
    {
        private readonly IDatabase _database;
        private readonly IBus _bus;
        private readonly IUserSession _userSession;

        public NotifyRequestedPaymentHandler(IDatabase database, IBus bus, IUserSession userSession)
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
                var job = _database.Single<Job>("SELECT * FROM Job WHERE JobNumber = @0", payment.JobNumber);


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
                        ObjectAccount = job.IsOutOfWarranty ? Configuration.WarrantyConstants.OverTwoYearLaborCode : Configuration.WarrantyConstants.UnderTwoYearLaborCode,
                        VarianceExplanation = ""
                    };

                _bus.Send(paymentRequest);
            }
        }
    }
}