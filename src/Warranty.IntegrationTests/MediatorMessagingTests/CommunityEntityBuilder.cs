using System;
using NPoco;
using Warranty.Core.Entities;

namespace Warranty.IntegrationTests.MediatorMessagingTests
{
    public class CommunityEntityBuilder : EntityBuilder<Community>
    {
        public CommunityEntityBuilder(IDatabase database)
            : base(database)
        {
        }

        public override Community GetSaved(Action<Community> action)
        {
            var comNum = Guid.NewGuid().ToString("n").Substring(1, 4);

            var entity = new Community
                {
                    CommunityNumber = comNum,
                    CreatedBy = "test",
                    CreatedDate = DateTime.UtcNow,
                };

            return Saved(entity, action);
        }
    }
}
