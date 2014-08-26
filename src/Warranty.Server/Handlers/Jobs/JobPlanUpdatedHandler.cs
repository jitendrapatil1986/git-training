using System;
using Accounting.Events.Job;
using NPoco;
using NServiceBus;
using Warranty.Core.Entities;
using Warranty.Server.Extensions;

namespace Warranty.Server.Handlers.Jobs
{
    public class JobPlanUpdatedHandler : IHandleMessages<JobPlanUpdated>
    {
        private readonly IDatabase _database;

        public JobPlanUpdatedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(JobPlanUpdated message)
        {
            using (_database)
            {
                var job = _database.SingleByJdeId<Job>(message.JDEId);

                job.PlanNumber = message.Plan;
                _database.Update(job);
            }
        }
    }
    public class JobAddedHandler : IHandleMessages<JobAdded>
    {
        private readonly IDatabase _database;

        public JobAddedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(JobAdded message)
        {
            using (_database)
            {
                var job = _database.SingleOrDefault<Job>(message.JDEId) ?? new Job {JdeIdentifier = message.JDEId};

                job.JobNumber = message.Job;
                job.CommunityId = _database.First<Guid>(
                    "SELECT CommunityId FROM Communities WHERE CommunityNumber = @0",
                    message.Community);
                job.PlanNumber = message.Plan;
                job.Elevation = message.Elevation;
                job.AddressLine = message.AddressLine1;
                job.City = message.City;
                job.StateCode = message.State;
                job.PostalCode = message.Zip;
                job.PlanName = message.Description;
                job.PlanType = message.JobType;

                _database.Insert(job);
            }
        }
    }
}