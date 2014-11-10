namespace Warranty.Core.Features.AddServiceCallPayment
{
    using System;

    public class AddPaymentOnHoldCommandHandlerResponse
    {
        public string NewStatusDisplayName { get; set; }
        public DateTime Date { get; set; }
    }
}