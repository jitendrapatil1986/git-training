using Accounting.Events.Job;
using NPoco;
using NServiceBus;
using Warranty.Core.Entities;
using Warranty.Server.Extensions;

namespace Warranty.Server.Handlers.Jobs
{
    public class JobWarrantyDateUpdatedHandler : IHandleMessages<JobWarrantyDateUpdated>
    {
        private readonly IDatabase _database;

        public JobWarrantyDateUpdatedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(JobWarrantyDateUpdated message)
        {
            using (_database)
            {
                var job = _database.SingleOrDefaultByJdeId<Job>(message.JDEId);
                if (job == null)
                    return;

                job.CloseDate = message.WarrantyDate;
                _database.Update(job);
            }
        }
    }
}
