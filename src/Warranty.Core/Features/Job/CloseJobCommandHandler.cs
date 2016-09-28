using System;
using log4net;
using Warranty.Core.Enumerations;
using Warranty.Core.Services;

namespace Warranty.Core.Features.Job
{
    public class CloseJobCommandHandler : ICommandHandler<CloseJobCommand>
    {
        private readonly IJobService _jobService;
        private readonly ITaskService _taskService;
        private readonly ILog _log;

        public CloseJobCommandHandler(IJobService jobService, ITaskService taskService, ILog log)
        {
            _jobService = jobService;
            _taskService = taskService;
            _log = log;
        }

        public void Handle(CloseJobCommand message)
        {
            var job = _jobService.GetJobById(message.JobId);
            job.CloseDate = message.CloseDate;
            _jobService.UpdateExistingJob(job);
            _log.InfoFormat("{0} Job is marked close.", job.JobNumber);

            try
            {
                _taskService.CreateTaskUnlessExists(job.JobId, TaskType.JobStage10JobClosed);
            }
            catch (InvalidOperationException ex)
            {
                _log.ErrorFormat("{0} for job {1}", ex.Message, job.JobNumber);
            }
        }
    }
}