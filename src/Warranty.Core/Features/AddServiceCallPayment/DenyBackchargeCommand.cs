namespace Warranty.Core.Features.AddServiceCallPayment
{
    using System;

    public class DenyBackchargeCommand : ICommand<DenyBackchargeCommandHandler.DenyBackchargeCommandHandlerResponse>
    {
        public Guid BackchargeId { get; set; }
        public string Message { get; set; }
    }
}