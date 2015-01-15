using Accounting.Events.Job;
using NPoco;
using NServiceBus;
using Warranty.Core.Entities;
using Warranty.Server.Extensions;

namespace Warranty.Server.Handlers.JobStages
{
    public class JobStageDateUpdatedHandler : IHandleMessages<JobStageDateUpdated>
    {
        private readonly IDatabase _database;

        public JobStageDateUpdatedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(JobStageDateUpdated message)
        {
            using (_database)
            {
                var jobStage = _database.SingleByJdeId<JobStage>(message.JDEId);
                jobStage.CompletionDate= message.ActualCompletionDate;
                _database.Update(jobStage);
            }
        }
    }
}