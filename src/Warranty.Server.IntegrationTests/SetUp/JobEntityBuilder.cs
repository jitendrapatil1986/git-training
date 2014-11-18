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
            var community = GetSaved<Community>();

            var entity = new Job{
                JdeIdentifier = "12345678",
                JobNumber = Guid.NewGuid().ToString().Substring(1,8),
                CreatedBy = "test",
                CreatedDate = DateTime.UtcNow,
                CommunityId = community.CommunityId,
            };

            return Saved(entity, action);
        }
    }
}