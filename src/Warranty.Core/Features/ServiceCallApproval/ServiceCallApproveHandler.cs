using Warranty.Core.ApprovalInfrastructure.Interfaces;
using Warranty.Core.Entities;

namespace Warranty.Core.Features.ServiceCallApproval
{
    public class ServiceCallApproveHandler : ICommandHandler<ServiceCallApproveCommand>
    {
        private readonly IApprovalService<ServiceCall> _approvalService;

        public ServiceCallApproveHandler(IApprovalService<ServiceCall> approvalService)
        {
            _approvalService = approvalService;
        }

        public void Handle(ServiceCallApproveCommand message)
        {
            _approvalService.Approve(message.ServiceCallId);
        }
    }
}
