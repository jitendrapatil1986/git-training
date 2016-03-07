using System;

namespace Warranty.Core.Features.ServiceCallApproval
{
    public class DeleteServiceCallCommand : ICommand
    {
        public Guid ServiceCallId { get; set; }
    }
}