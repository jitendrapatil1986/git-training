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

        public JobStageUpdatedHandler(IDatabase database, ITaskService taskService)
        {
            _database = database;
            _taskService = taskService;
        }

        public void Handle(JobStageUpdated message)
        {
            using (_database)
            {
                var job = _database.SingleByJdeId<Job>(message.Job);

                job.Stage = Convert.ToInt32(Convert.ToDecimal(message.Stage));
                _database.Update(job);
                _taskService.CreateTasks(job.JobId);
            }
        }
    }
}