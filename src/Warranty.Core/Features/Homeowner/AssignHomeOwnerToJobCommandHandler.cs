using Warranty.Core.Services;
using Warranty.Server.Handlers.Jobs;

namespace Warranty.Core.Features.Homeowner
{
    public class AssignHomeOwnerToJobCommandHandler : ICommandHandler<AssignHomeOwnerToJobCommand>
    {
        private readonly IJobService _jobService;
        private readonly IHomeOwnerService _homeOwnerService;

        public AssignHomeOwnerToJobCommandHandler(IHomeOwnerService homeOwnerService, IJobService jobService)
        {
            _homeOwnerService = homeOwnerService;
            _jobService = jobService;
        }

        public void Handle(AssignHomeOwnerToJobCommand message)
        {
            var job = _jobService.GetJobById(message.JobId);
            var homeOwner = _homeOwnerService.GetByHomeOwnerId(message.HomeOwnerId);

            _homeOwnerService.AssignToJob(homeOwner, job);
        }
    }
}