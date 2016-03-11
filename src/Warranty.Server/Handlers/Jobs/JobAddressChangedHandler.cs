using System;
using Land.Events;
using NServiceBus;
using Warranty.Core.Services;

namespace Warranty.Server.Handlers.Jobs
{
    public class JobAddressChangedHandler : IHandleMessages<LotAddressChanged>
    {
        private readonly IJobService _jobService;

        public JobAddressChangedHandler(IJobService jobService)
        {
            if (jobService == null)
                throw new ArgumentNullException("jobService");

            _jobService = jobService;
        }

        public void Handle(LotAddressChanged message)
        {
            var job = _jobService.GetJobByNumber(message.JdeIdentifier);

            //We may not have the job in warranty yet so just return in that case
            if (job == null)
                return;

            job.AddressLine = string.Format("{0} {1}", message.NewStreetNumber, message.NewStreetName);

            _jobService.Save(job);
        }
    }
}