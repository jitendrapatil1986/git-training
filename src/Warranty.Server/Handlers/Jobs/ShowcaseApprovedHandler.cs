using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPoco;
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
            _jobService = jobService;
            _taskService = taskService;
        }

        public void Handle(ShowcaseApproved message)
        {
            var job = _jobService.GetJobByNumber(message.Showcase.JobNumber);

            if (job == null)
            {
                job = _jobService.CreateJobAndInsert(message.Showcase);
            }
            else
            {
                _jobService.UpdateExistingJob(job, message.Showcase);
            }

            if (message.Showcase.Stage == 7)
            {
               _taskService.CreateTaskUnlessExists(job.JobId, TaskType.JobStage7); 
            }
        }
    }
}
