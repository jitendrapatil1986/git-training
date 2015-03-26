using System;
using Accounting.Events.Job;
using NPoco;
using NServiceBus;
using Warranty.Core.Entities;
using Warranty.Core.Extensions;
using Warranty.Server.Extensions;

namespace Warranty.Server.Handlers.Jobs
{
    public class JobHomeBuyerNameUpdatedHandler : IHandleMessages<JobHomeBuyerNameUpdated>
    {
        private readonly IDatabase _database;

        public JobHomeBuyerNameUpdatedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(JobHomeBuyerNameUpdated message)
        {
            using (_database)
            {
                var job = _database.SingleByJdeId<Job>(message.JDEId);

                if (job == null)
                    throw new Exception(string.Format("Job {0} not found in the Warranty database.", message.JDEId));

                var homeOwner = _database.SingleById<HomeOwner>(job.CurrentHomeOwnerId);

                if (homeOwner == null)
                    throw new Exception(string.Format("Job {0} ({1}) has no current homeowner.", job.JobId, job.JdeIdentifier));

                homeOwner.HomeOwnerName = (message.BuyerName.IsNullOrEmpty()) ? null : message.BuyerName;
                _database.Update(homeOwner);
            }
        }
    }
}