using System.Linq;
using Accounting.Events.Showcase;
using NPoco;
using NServiceBus;
using Warranty.Core.DataAccess;
using Warranty.Core.Entities;
using Warranty.Server.Extensions;

namespace Warranty.Server.Handlers.Jobs
{
    public class ModelAddedHandler : IHandleMessages<ModelAdded>
    {
        private readonly SqlServerDatabase _database;

        public ModelAddedHandler(IDatabase database)
        {
            _database = (SqlServerDatabase) database;
        }

        public void Handle(ModelAdded message)
        {
            using (_database)
            {
                var job = _database.SingleOrDefaultByJdeId<Job>(message.JDEId);

                if (job == null)
                {
                    var communityId =
                        _database.FetchWhere<Community>(c => c.CommunityNumber == message.Community.Substring(0, 4))
                            .Select(c => c.CommunityId)
                            .Single();

                    job = new Job
                    {
                        JdeIdentifier = message.JDEId,
                        JobNumber = message.Job,
                        CommunityId = communityId,
                        PlanNumber = message.Plan,
                        Elevation = message.Elevation,
                        AddressLine = message.AddressLine1,
                        City = message.City,
                        StateCode = message.State,
                        PostalCode = message.Zip,
                        PlanType = message.Plan,
                    };

                    _database.Insert(job);
                }
            }
        }
    }
}