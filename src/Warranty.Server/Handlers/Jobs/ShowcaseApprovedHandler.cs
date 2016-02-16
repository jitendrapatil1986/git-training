using System;
using NServiceBus;
using TIPS.Events.JobEvents;
using Warranty.Core.Enumerations;
using Warranty.Core.Services;

namespace Warranty.Server.Handlers.Jobs
{
    public class ShowcaseApprovedHandler : IHandleMessages<ShowcaseApproved>
    {
        private IJobService _jobService;
        private ITaskService _taskService;
        private IHomeOwnerService _homeOwnerService;

        public ShowcaseApprovedHandler(IJobService jobService, ITaskService taskService, IHomeOwnerService homeOwnerService)
        {
            if (jobService == null)
                throw new ArgumentNullException("jobService");
            if (taskService == null)
                throw new ArgumentNullException("taskService");
            if (homeOwnerService == null)
                throw new ArgumentNullException("homeOwnerService");

            _jobService = jobService;
            _taskService = taskService;
            _homeOwnerService = homeOwnerService;
        }

        public void Handle(ShowcaseApproved message)
        {
            if(message.Showcase == null)
                throw new ArgumentException("Showcase is null");
            if (string.IsNullOrWhiteSpace(message.Showcase.JobNumber))
                throw new ArgumentException("JobNumber is null or whitespace");

            var job = _jobService.GetJobByNumber(message.Showcase.JobNumber);

            if (job == null)
            {
                job = _jobService.CreateJob(message.Showcase);
            }
            else
            {
                if (job.CurrentHomeOwnerId != null || _homeOwnerService.GetHomeOwnerByJobNumber(job.JobNumber) != null)
                    throw new InvalidOperationException(string.Format("Homeowner exists on job number {0}", job.JobNumber));

                _jobService.UpdateExistingJob(job, message.Showcase);
            }
            _taskService.CreateTasks(job.JobId);
        }
    }
}
