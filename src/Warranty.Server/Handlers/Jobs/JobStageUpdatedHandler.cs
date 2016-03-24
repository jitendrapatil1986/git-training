using log4net;
using Warranty.Core.Services;

namespace Warranty.Server.Handlers.Jobs
{
    using System;
    using Construction.Events.Jobs;
    using Core.Entities;
    using Extensions;
    using NPoco;
    using NServiceBus;

    public class JobStageUpdatedHandler : IHandleMessages<JobStageUpdated>
    {
        private readonly IDatabase _database;
        private readonly ITaskService _taskService;
        private readonly ILog _log;

        public JobStageUpdatedHandler(IDatabase database, ITaskService taskService, ILog log)
        {
            _database = database;
            _taskService = taskService;
            _log = log;
        }

        public void Handle(JobStageUpdated message)
        {
            using (_database)
            {
                var job = _database.SingleOrDefaultByJdeId<Job>(message.Job);
                if (job == null)
                {
                    _log.WarnFormat("Could not update stage for job {0} because it does not exist locally", message.Job);  
                    return;  
                }

                job.Stage = Convert.ToInt32(Convert.ToDecimal(message.Stage));
                _database.Update(job);
                _taskService.CreateTasks(job.JobId);
            }
        }
    }
}