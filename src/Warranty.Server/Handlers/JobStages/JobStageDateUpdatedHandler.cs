using System;
using System.Linq;
using Accounting.Events.Job;
using log4net;
using NPoco;
using NServiceBus;
using Warranty.Core.Entities;
using Warranty.Server.Extensions;

namespace Warranty.Server.Handlers.JobStages
{
    public class JobStageDateUpdatedHandler : IHandleMessages<JobStageDateUpdated>
    {
        private readonly IDatabase _database;
        private readonly ILog _log;

        public JobStageDateUpdatedHandler(IDatabase database, ILog log)
        {
            _database = database;
            _log = log;
        }

        public void Handle(JobStageDateUpdated message)
        {
            using (_database)
            {
                var jobStage = _database.SingleOrDefaultByJdeId<JobStage>(message.JDEId);
                if (jobStage != null)
                {
                    jobStage.CompletionDate = message.ActualCompletionDate;
                    _database.Update(jobStage);
                    return;
                }

                var split = message.JDEId.Split('/');
                var jobNumber = split[0];
                var stage = Convert.ToInt32(split[1]);

                var job = _database.FetchBy<Job>(sql => sql.Where(j => j.JobNumber == jobNumber)).SingleOrDefault();
                if (job == null)
                {
                    _log.ErrorFormat("Cannot update JobStage for job {0} - which was not found", jobNumber);
                    return;
                    // This should be refactored to pull job information from TIPS? when it's not found
                }

                jobStage = new JobStage
                {
                    JdeIdentifier = message.JDEId,
                    Stage = stage,
                    JobId = job.JobId,
                    CompletionDate = message.ActualCompletionDate
                };
                _database.Save<JobStage>(jobStage);
            }
        }
    }
}