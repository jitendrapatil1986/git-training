using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using NServiceBus;
using TIPS.Events.JobEvents;
using Warranty.Core.Services;

namespace Warranty.Server.Handlers.Jobs
{
    public class ShowcasePlanUpdateApprovedHandler: IHandleMessages<ShowcasePlanUpdateApproved>
    {
        private IJobService _jobService;
        private readonly ILog _log = LogManager.GetLogger(typeof(ShowcasePlanUpdateApprovedHandler));

        public ShowcasePlanUpdateApprovedHandler(IJobService jobService)
        {
            if (jobService == null)
                throw new ArgumentNullException("jobService");

            _jobService = jobService;
        }

        public void Handle(ShowcasePlanUpdateApproved message)
        {
            if(string.IsNullOrWhiteSpace(message.JobNumber))
                throw new InvalidOperationException("JobNumber is null or whitespace");

            var showcase = _jobService.GetJobByNumber(message.JobNumber);
            if(showcase == null)
                throw new InvalidOperationException("Showcase does not exist in Warranty. Can not update elevation.");

            showcase.Elevation = message.Elevation;
            showcase.PlanName = message.PlanName;
            showcase.PlanNumber = message.PlanNumber;

            _log.Info(string.Format("Updating job '{0}'",showcase.JobId));
            _jobService.Save(showcase);
        }
    }
}
