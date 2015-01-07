namespace Warranty.Core.Features.AddServiceCallPayment
{
    using System;
    using Enumerations;

    public class AddPaymentCommandDto
    {
        public Guid PaymentId { get; set; }
        public WarrantyCostCode CostCode { get; set; }
    }
}