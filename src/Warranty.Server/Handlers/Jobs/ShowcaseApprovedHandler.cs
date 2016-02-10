using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPoco;
using NServiceBus;
using TIPS.Events.JobEvents;
using Warranty.Core.Services;

namespace Warranty.Server.Handlers.Jobs
{
    public class ShowcaseApprovedHandler : IHandleMessages<ShowcaseApproved>
    {
        private IJobService _jobService;

        public ShowcaseApprovedHandler(IJobService jobService)
        {
            _jobService = jobService;
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
                
            }
        }
    }
}
