using Accounting.Events.Job;
using NPoco;
using NServiceBus;
using Warranty.Core.Entities;
using Warranty.Server.Extensions;

namespace Warranty.Server.Handlers.Jobs
{
    public class JobElevationUpdatedHandler : IHandleMessages<JobElevationUpdated>
    {
        private readonly IDatabase _database;

        public JobElevationUpdatedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(JobElevationUpdated message)
        {
            using (_database)
            {
                var job = _database.SingleByJdeId<Job>(message.JDEId);

                job.Elevation = message.Elevation;
                _database.Update(job);
            }
        }
    }
}