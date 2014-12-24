namespace Warranty.Core.Features.AddServiceCallPayment
{
    using System;

    public class DeletePaymentCommand : ICommand
    {
        public Guid PaymentId { get; set; }
    }
}