using System;
using System.Globalization;
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
            var r = new Random();
            var jobNum = r.Next(12345678, 88889999).ToString(CultureInfo.InvariantCulture);
            using (_database)
            {
                Job existing = null;
                while ((existing = _database.SingleOrDefault<Job>(string.Format("WHERE JobNumber = {0}", jobNum))) != null)
                {
                    jobNum = r.Next(12345678, 88889999).ToString(CultureInfo.InvariantCulture);
                }
            }
            var entity = new Job{
                JdeIdentifier = jobNum,
                JobNumber = jobNum,
                CreatedBy = "test",
                CreatedDate = DateTime.UtcNow,
                CommunityId = community.CommunityId,
            };

            return Saved(entity, action);
        }

        public new Job GetRandom()
        {
            
            return new Job();
        }
    }
}