using System.Linq;
using Accounting.Events.Job;
using NPoco;
using NServiceBus;
using Warranty.Core.DataAccess;
using Warranty.Core.Entities;

namespace Warranty.Server.Handlers.Jobs
{
    public class JobAddedHandler : IHandleMessages<JobAdded>
    {
        private readonly SqlServerDatabase _database;

        public JobAddedHandler(IDatabase database)
        {
            _database = (SqlServerDatabase) database;
        }

        public void Handle(JobAdded message)
        {
            using (_database)
            {
                var job = _database.SingleOrDefault<Job>(message.JDEId) ?? new Job {JdeIdentifier = message.JDEId};
                var communityId =
                    _database.FetchWhere<Community>(c => c.CommunityNumber == message.Community)
                        .Select(c => c.CommunityId)
                        .Single();

                job.JobNumber = message.Job;
                job.CommunityId = communityId;
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