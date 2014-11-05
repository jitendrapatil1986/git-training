namespace Warranty.Core.Features.AddServiceCallPayment
{
    using System;

    public class DeleteServiceCallLineItemPaymentCommand : ICommand
    {
        public Guid PaymentId { get; set; }
    }
}