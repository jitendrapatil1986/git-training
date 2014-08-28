using System;

namespace Warranty.Core.Features.ServiceCallApproval
{
    public class ServiceCallApproveCommand : ICommand
    {
        public Guid ServiceCallId { get; set; }
    }
}