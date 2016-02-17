using System;
using log4net;
using NServiceBus;
using TIPS.Events.JobEvents;
using Warranty.Core.Services;

namespace Warranty.Server.Handlers.Jobs
{
    public class ShowcaseElevationUpdateApprovedHandler: IHandleMessages<ShowcaseElevationUpdateApproved>
    {
        private IJobService _jobService;
        private readonly ILog _log = LogManager.GetLogger(typeof(JobSaleApprovedHandler));

        public ShowcaseElevationUpdateApprovedHandler(IJobService jobService)
        {
            if (jobService == null)
                throw new ArgumentNullException("jobService");

            _jobService = jobService;
        }

        public void Handle(ShowcaseElevationUpdateApproved message)
        {
            if(string.IsNullOrWhiteSpace(message.JobNumber))
                throw new InvalidOperationException("JobNumber is null or whitespace");
            if(string.IsNullOrWhiteSpace(message.Elevation))
                throw new InvalidOperationException("Elevation is null or whitespace");

            var showcase = _jobService.GetJobByNumber(message.JobNumber);
            if(showcase == null)
                throw new InvalidOperationException("Showcase does not exist in Warranty. Can not update elevation.");

            _log.Info(string.Format("Updating job '{0}' from elevation '{1}' to elevation '{2}'", showcase.JobId, showcase.Elevation, message.Elevation));
            showcase.Elevation = message.Elevation;
            _jobService.Save(showcase);
        }
    }
}
