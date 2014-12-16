namespace Warranty.Core.Features.AddServiceCallPayment
{
    using System;

    public class ApprovePaymentCommand : ICommand<string>
    {
        public Guid PaymentId { get; set; }
    }
}