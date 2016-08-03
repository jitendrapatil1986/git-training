using Warranty.Core.Enumerations;
using Warranty.Core.Services;

namespace Warranty.Core.Features.Job
{
    public class CloseJobCommandHandler : ICommandHandler<CloseJobCommand>
    {
        private readonly IJobService _jobService;
        private readonly ITaskService _taskService;

        public CloseJobCommandHandler(IJobService jobService, ITaskService taskService)
        {
            _jobService = jobService;
            _taskService = taskService;
        }

        public void Handle(CloseJobCommand message)
        {
            var job = _jobService.GetJobById(message.JobId);
            job.CloseDate = message.CloseDate;
            _jobService.UpdateExistingJob(job);

            _taskService.CreateTaskUnlessExists(job.JobId, TaskType.JobStage10JobClosed);
        }
    }
}