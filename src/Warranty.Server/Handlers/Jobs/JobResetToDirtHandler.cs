using Accounting.Events.Job;
using NPoco;
using NServiceBus;
using Warranty.Core.Entities;
using Warranty.Server.Extensions;

namespace Warranty.Server.Handlers.Jobs
{
    public class JobResetToDirtHandler : IHandleMessages<JobResetToDirt>
    {
        private readonly IDatabase _database;

        public JobResetToDirtHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(JobResetToDirt message)
        {
            using (_database)
            {
                var job = _database.SingleByJdeId<Job>(message.JDEId);

                job.CloseDate = null;
                job.CurrentHomeOwnerId = null;
                job.PlanType = null;
                job.PlanTypeDescription = null;
                job.PlanName = null;
                job.PlanNumber = null;
                job.Elevation = null;
                job.Swing = null;
                job.WarrantyExpirationDate = null;
                job.BuilderEmployeeId = null;
                job.SalesConsultantEmployeeId = null;
                _database.Update(job);
            }
        }
    }
}