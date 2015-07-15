using System;
using System.Linq;
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
                var jobStage = _database.SingleOrDefaultByJdeId<JobStage>(message.JDEId);
                if (jobStage == null)
                {
                    var split = message.JDEId.Split('/');
                    var jobNumber = split[0];
                    var stage = Convert.ToInt32(split[1]);
                    var job = _database.FetchBy<Job>(sql => sql.Where(j => j.JobNumber == jobNumber)).Single();
                    jobStage = new JobStage {JdeIdentifier = message.JDEId, Stage = stage, JobId = job.JobId, CompletionDate = message.ActualCompletionDate};
                    _database.Save<JobStage>(jobStage);
                }
                else
                {
                    jobStage.CompletionDate = message.ActualCompletionDate;
                    _database.Update(jobStage);
                }
            }
        }
    }
}