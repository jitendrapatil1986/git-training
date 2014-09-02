using System;
using NPoco;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.SetUp
{
    public class JobStageEntityBuilder : EntityBuilder<JobStage>
    {
        public JobStageEntityBuilder(IDatabase database)
            : base(database)
        {
        }

        public override JobStage GetSaved(Action<JobStage> action)
        {
            var job = GetSaved<Job>();

            var entity = new JobStage{
                JdeIdentifier = "12345678/001",
                JobId = job.JobId,
                CreatedBy = "test",
                CreatedDate = DateTime.UtcNow,
                Stage = 1,
                CompletionDate = null,
            };

            return Saved(entity, action);
        }
    }
}