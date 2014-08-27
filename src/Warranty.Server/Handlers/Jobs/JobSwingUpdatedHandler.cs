using Accounting.Events.Job;
using NPoco;
using NServiceBus;
using Warranty.Core.Entities;
using Warranty.Server.Extensions;

namespace Warranty.Server.Handlers.Jobs
{
    public class JobSwingUpdatedHandler : IHandleMessages<JobSwingUpdated>
    {
        private readonly IDatabase _database;

        public JobSwingUpdatedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(JobSwingUpdated message)
        {
            using (_database)
            {
                var job = _database.SingleByJdeId<Job>(message.JDEId);

                job.Swing = message.Swing;
                _database.Update(job);
            }
        }
    }
}