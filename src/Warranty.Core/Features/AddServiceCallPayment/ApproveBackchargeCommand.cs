namespace Warranty.Core.Features.AddServiceCallPayment
{
    using System;

    public class ApproveBackchargeCommand : ICommand<string>
    {
        public Guid BackchargeId { get; set; }
    }
}