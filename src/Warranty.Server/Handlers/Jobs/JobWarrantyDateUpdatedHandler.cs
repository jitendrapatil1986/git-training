using Accounting.Events.Job;
using log4net;
using NPoco;
using NServiceBus;
using Warranty.Core.Entities;
using Warranty.Server.Extensions;

namespace Warranty.Server.Handlers.Jobs
{
    public class JobWarrantyDateUpdatedHandler : IHandleMessages<JobWarrantyDateUpdated>
    {
        private readonly IDatabase _database;
        private readonly ILog _log;

        public JobWarrantyDateUpdatedHandler(IDatabase database, ILog log)
        {
            _database = database;
            _log = log;
        }

        public void Handle(JobWarrantyDateUpdated message)
        {
            using (_database)
            {
                var job = _database.SingleOrDefaultByJdeId<Job>(message.JDEId);
                if (job == null)
                {
                    _log.ErrorFormat("Cannoy update warranty date for job {0} because it does not exist locally", message.JDEId);
                    return;
                }

                job.CloseDate = message.WarrantyDate;
                job.WarrantyExpirationDate = message.WarrantyDate.AddYears(10);
                _database.Update(job);
            }
        }
    }
}
