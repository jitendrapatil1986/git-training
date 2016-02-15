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

        public ShowcaseApprovedHandler(IJobService jobService, ITaskService taskService)
        {
            if (jobService == null)
                throw new ArgumentNullException("jobService");
            if (taskService == null)
                throw new ArgumentNullException("taskService");

            _jobService = jobService;
            _taskService = taskService;
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
                _jobService.UpdateExistingJob(job, message.Showcase);
            }
            _taskService.CreateTasks(job.JobId);
        }
    }
}
