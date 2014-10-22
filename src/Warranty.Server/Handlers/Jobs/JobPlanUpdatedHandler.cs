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
}