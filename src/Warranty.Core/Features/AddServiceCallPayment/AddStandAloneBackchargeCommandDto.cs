namespace Warranty.Core.Features.AddServiceCallPayment
{
    using System;
    using Enumerations;

    public class AddStandAloneBackchargeCommandDto
    {
        public Guid BackchargeId { get; set; }
        public WarrantyCostCode CostCode { get; set; } 
    }
}