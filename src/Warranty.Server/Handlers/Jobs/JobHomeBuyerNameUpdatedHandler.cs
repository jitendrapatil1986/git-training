using System;
using Accounting.Events.Job;
using NPoco;
using NServiceBus;
using Warranty.Core.Entities;
using Warranty.Server.Extensions;

namespace Warranty.Server.Handlers.Jobs
{
    public class JobHomeBuyerNameUpdatedHandler : IHandleMessages<JobHomeBuyerNameUpdated>
    {
        private readonly IDatabase _database;

        public JobHomeBuyerNameUpdatedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(JobHomeBuyerNameUpdated message)
        {
            using (_database)
            {
                var job = _database.SingleByJdeId<Job>(message.JDEId);
                var homeOwner = _database.SingleById<HomeOwner>(job.CurrentHomeOwnerId);

                homeOwner.HomeOwnerNumber = Convert.ToInt32(message.HomeBuyerNumber);
                homeOwner.HomeOwnerName = message.BuyerName;
                _database.Update(homeOwner);
            }
        }
    }
}