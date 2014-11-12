namespace Warranty.Core.Features.AddServiceCallPayment
{
    using System;

    public class HoldPaymentCommand : ICommand<HoldPaymentCommandHandler.HoldPaymentCommandHandlerResponse>
    {
        public Guid PaymentId { get; set; }
        public string Message { get; set; }
    }

}