using System;
using System.Linq;
using Accounting.Events.Job;
using NPoco;
using NServiceBus;
using Warranty.Core.Entities;
using Warranty.Server.Extensions;

namespace Warranty.Server.Handlers.JobStages
{
    public class JobStageDateAddedHandler : IHandleMessages<JobStageDateAdded>
    {
        private readonly IDatabase _database;

        public JobStageDateAddedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(JobStageDateAdded message)
        {
            using (_database)
            {
                var stage = Convert.ToInt32(Convert.ToDecimal(message.JobStage));

                var jobStage = _database.SingleOrDefaultByJdeId<JobStage>(message.JDEId)
                               ?? new JobStage {JdeIdentifier = message.JDEId};

                var job = _database.FetchBy<Job>(sql => sql.Where(j => j.JobNumber == message.Job)).Single();

                jobStage.JobId = job.JobId;
                jobStage.Stage = stage;
                jobStage.CompletionDate = message.ActualCompletionDate;
                _database.Save<JobStage>(jobStage);
            }
        }
    }
}