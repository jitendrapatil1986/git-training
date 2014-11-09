namespace Warranty.Core.Features.AddServiceCallPayment
{
    using System;

    public class AddPaymentOnHoldCommand : ICommand
    {
        public Guid PaymentId { get; set; }
    }
}