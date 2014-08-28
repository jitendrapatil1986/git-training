using Warranty.Core.ApprovalInfrastructure.Interfaces;
using Warranty.Core.Entities;

namespace Warranty.Core.Features.ServiceCallApproval
{
    public class ServiceCallDenyHandler : ICommandHandler<ServiceCallDenyCommand>
    {
        private readonly IApprovalService<ServiceCall> _approvalService;

        public ServiceCallDenyHandler(IApprovalService<ServiceCall> approvalService)
        {
            _approvalService = approvalService;
        }

        public void Handle(ServiceCallDenyCommand message)
        {
            _approvalService.Deny(message.ServiceCallId);
        }
    }
}
