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

                HomeOwner homeOwner = null;
                if (job.CurrentHomeOwnerId != null)
                    homeOwner = _database.SingleOrDefaultById<HomeOwner>(job.CurrentHomeOwnerId);

                if (homeOwner == null)
                {
                    homeOwner = new HomeOwner {HomeOwnerName = message.BuyerName, JobId = job.JobId};
                }
                else
                {
                    homeOwner.HomeOwnerName = (message.BuyerName.IsNullOrEmpty()) ? null : message.BuyerName;
                }

                _database.Update(homeOwner);
            }
        }
    }
}