using Accounting.Events.Job;
using NPoco;
using NServiceBus;
using Warranty.Core.Entities;
using Warranty.Server.Extensions;

namespace Warranty.Server.Handlers.Jobs
{
    public class JobClosedHandler : IHandleMessages<JobClosed>
    {
        private readonly IDatabase _database;

        public JobClosedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(JobClosed message)
        {
            using (_database)
            {
                var job = _database.SingleByJdeId<Job>(message.JDEId);

                job.CloseDate = message.CloseDate;
                _database.Update(job);
            }
        }
    }
}