namespace Warranty.Core.Features.AddServiceCallPayment
{
    using System;

    public class HoldBackchargeCommand : ICommand<HoldBackchargeCommandHandler.HoldBackchargeCommandHandlerResponse>
    {
        public Guid BackchargeId { get; set; }
        public string Message { get; set; }
    }
}