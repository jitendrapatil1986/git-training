using System;

namespace Warranty.Core.Features.ServiceCallApproval
{
    public class ServiceCallDenyCommand : ICommand
    {
        public Guid ServiceCallId { get; set; }
    }
}