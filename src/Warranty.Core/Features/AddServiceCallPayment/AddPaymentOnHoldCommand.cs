namespace Warranty.Core.Features.AddServiceCallPayment
{
    using System;

    public class AddPaymentOnHoldCommand : ICommand<AddPaymentOnHoldCommandHandlerResponse>
    {
        public Guid PaymentId { get; set; }
        public string Message { get; set; }
    }

}