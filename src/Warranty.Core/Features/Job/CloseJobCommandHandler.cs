using log4net;
using Warranty.Core.Enumerations;
using Warranty.Core.Services;

namespace Warranty.Core.Features.Job
{
    public class CloseJobCommandHandler : ICommandHandler<CloseJobCommand>
    {
        private readonly IJobService _jobService;
        private readonly ITaskService _taskService;
        private readonly IEmployeeService _employeeService;
        private readonly ILog _log;

        public CloseJobCommandHandler(IJobService jobService, ITaskService taskService, IEmployeeService employeeService, ILog log)
        {
            _jobService = jobService;
            _taskService = taskService;
            _employeeService = employeeService;
            _log = log;
        }

        public void Handle(CloseJobCommand message)
        {
            var job = _jobService.GetJobById(message.JobId);
            job.CloseDate = message.CloseDate;
            _jobService.UpdateExistingJob(job);
            _log.InfoFormat("{0} Job is marked close.", job.JobNumber);

            var hasWsr = _employeeService.GetWsrByJobId(job.JobId) != null;

            if (!hasWsr)
            {
                _log.ErrorFormat("{0} Job doesn't have a WSR assigned yet, so failed to create a task.", job.JobNumber);
                return;
            }

            _taskService.CreateTaskUnlessExists(job.JobId, TaskType.JobStage10JobClosed);
        }
    }
}