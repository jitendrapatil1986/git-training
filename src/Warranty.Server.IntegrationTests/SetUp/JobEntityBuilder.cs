using System;
using NPoco;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.SetUp
{
    public class JobEntityBuilder : EntityBuilder<Job>
    {
        public JobEntityBuilder(IDatabase database)
            : base(database)
        {
        }

        public override Job GetSaved(Action<Job> action)
        {
            var entity = new Job{
                JdeIdentifier = "12345678",
                JobNumber = "12345678",
                CreatedBy = "test",
                CreatedDate = DateTime.UtcNow,
            };

            return Saved(entity, action);
        }
    }
}