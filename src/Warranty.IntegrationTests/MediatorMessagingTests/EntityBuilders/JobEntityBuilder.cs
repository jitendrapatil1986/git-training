namespace Warranty.IntegrationTests.MediatorMessagingTests.EntityBuilders
{
    using System;
    using System.Globalization;
    using NPoco;
    using Warranty.Core.Entities;

    public class JobEntityBuilder : EntityBuilder<Job>
    {
        public JobEntityBuilder(IDatabase database)
            : base(database)
        {
        }

        public override Job GetSaved(Action<Job> action)
        {
            var community = GetSaved<Community>();
            var r = new Random();
            var jobNum = r.Next(12345678, 88889999).ToString(CultureInfo.InvariantCulture);

            var entity = new Job
            {
                JdeIdentifier = jobNum,
                JobNumber = jobNum,
                CreatedBy = "test",
                CreatedDate = DateTime.UtcNow,
                CommunityId = community.CommunityId,
            };

            return Saved(entity, action);
        }
    }
}