namespace Warranty.Core.Features.AddServiceCallPayment
{
    using System;

    public class DeleteBackchargeCommand : ICommand
    {
        public Guid BackchargeId { get; set; }
    }
}