using Accounting.Events.Job;
using NPoco;
using NServiceBus;
using Warranty.Core.Entities;
using Warranty.Server.Extensions;

namespace Warranty.Server.Handlers.Jobs
{
    using Core.TaskInfrastructure.Tasks;

    public class JobClosedHandler : IHandleMessages<JobClosed>
    {
        private readonly IDatabase _database;
        private readonly JobClosedContactHomeownerTask _jobClosedContactHomeownerTask;

        public JobClosedHandler(IDatabase database, JobClosedContactHomeownerTask jobClosedContactHomeownerTask)
        {
            _database = database;
            _jobClosedContactHomeownerTask = jobClosedContactHomeownerTask;
        }

        public void Handle(JobClosed message)
        {
            using (_database)
            {
                var job = _database.SingleByJdeId<Job>(message.JDEId);

                job.CloseDate = message.CloseDate;
                _database.Update(job);
                _jobClosedContactHomeownerTask.Create(job);
            }
        }
    }
}