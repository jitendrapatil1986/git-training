namespace Warranty.Core.Features.AddServiceCallPayment
{
    using System;
    using Enumerations;

    public class AddPaymentCommandDto
    {
        public Guid PaymentId { get; set; }
        public Guid? BackchargeId { get; set; }
        public WarrantyCostCode CostCode { get; set; }
    }
}