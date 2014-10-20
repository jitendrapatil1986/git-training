namespace Warranty.Server.Handlers.Jobs
{
    using System;
    using Accounting.Events.Job;
    using Core.Entities;
    using Core.TaskInfrastructure.Tasks;
    using Extensions;
    using NPoco;
    using NServiceBus;

    public class JobStageUpdatedHandler : IHandleMessages<JobStageUpdated>
    {
        private readonly IDatabase _database;
        private readonly JobStageUpdatedContactBuilderTask _jobStageUpdatedContactBuilderTask;

        public JobStageUpdatedHandler(IDatabase database, JobStageUpdatedContactBuilderTask jobStageUpdatedContactBuilderTask)
        {
            _database = database;
            _jobStageUpdatedContactBuilderTask = jobStageUpdatedContactBuilderTask;
        }

        public void Handle(JobStageUpdated message)
        {
            using (_database)
            {
                var job = _database.SingleByJdeId<Job>(message.JDEId);

                job.Stage = Convert.ToInt32(Convert.ToDecimal(message.Stage));
                _database.Update(job);
                _jobStageUpdatedContactBuilderTask.Create(job);
            }
        }
    }
}