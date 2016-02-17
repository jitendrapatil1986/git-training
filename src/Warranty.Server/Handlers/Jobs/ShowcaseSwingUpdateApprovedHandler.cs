using System;
using log4net;
using NServiceBus;
using TIPS.Events.JobEvents;
using Warranty.Core.Services;

namespace Warranty.Server.Handlers.Jobs
{
    public class ShowcaseSwingUpdateApprovedHandler : IHandleMessages<ShowcaseSwingUpdateApproved>
    {
        private IJobService _jobService;
        private readonly ILog _log = LogManager.GetLogger(typeof (ShowcaseSwingUpdateApprovedHandler));

        public ShowcaseSwingUpdateApprovedHandler(IJobService jobService)
        {
            _jobService = jobService;
        }

        public void Handle(ShowcaseSwingUpdateApproved message)
        {
            if (string.IsNullOrWhiteSpace(message.JobNumber))
                throw new InvalidOperationException("JobNumber is null or whitespace.");

            var showcase = _jobService.GetJobByNumber(message.JobNumber);
            if (showcase == null)
                throw new InvalidOperationException("Showcase doesn't exist in Warranty. Can not update.");

            showcase.Swing = message.Swing;
            _jobService.Save(showcase);
            _log.Info(string.Format("Updated showcase JobID '{0}'", showcase.JobId));
        }
    }
}