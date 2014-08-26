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
                var job = _database.SingleByJdeId<Job>(message.JDEId);

                job.CloseDate = message.WarrantyDate;
                job.WarrantyExpirationDate = message.WarrantyDate.AddYears(10);
                _database.Update(job);
            }
        }
    }
}
