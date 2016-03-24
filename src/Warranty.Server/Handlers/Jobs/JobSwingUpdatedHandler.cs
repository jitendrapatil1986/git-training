using Accounting.Events.Job;
using log4net;
using NPoco;
using NServiceBus;
using Warranty.Core.Entities;
using Warranty.Server.Extensions;

namespace Warranty.Server.Handlers.Jobs
{
    public class JobSwingUpdatedHandler : IHandleMessages<JobSwingUpdated>
    {
        private readonly IDatabase _database;
        private readonly ILog _log;

        public JobSwingUpdatedHandler(IDatabase database, ILog log)
        {
            _database = database;
            _log = log;
        }

        public void Handle(JobSwingUpdated message)
        {
            using (_database)
            {
                var job = _database.SingleOrDefaultByJdeId<Job>(message.JDEId);

                if (job == null)
                {
                    _log.WarnFormat("Could not update swing for job {0} because it does not exist locally", message.JDEId);
                    return;
                }

                job.Swing = message.Swing;
                _database.Update(job);
            }
        }
    }
}