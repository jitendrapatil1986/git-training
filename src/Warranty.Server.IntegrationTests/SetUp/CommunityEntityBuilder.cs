using System;
using NPoco;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.SetUp
{
    public class CommunityEntityBuilder : EntityBuilder<Community>
    {
        public CommunityEntityBuilder(IDatabase database)
            : base(database)
        {
        }

        public override Community GetSaved(Action<Community> action)
        {
            var entity = new Community
            {
                CommunityNumber = "1234",
                CreatedBy = "test",
                CreatedDate = DateTime.UtcNow,
            };

            return Saved(entity, action);
        }
    }
}