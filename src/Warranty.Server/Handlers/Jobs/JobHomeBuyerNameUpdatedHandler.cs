using System;
using Accounting.Events.Job;
using NPoco;
using NServiceBus;
using Warranty.Core.Entities;
using Warranty.Core.Extensions;
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

                var ownerNumber = Convert.ToInt32(message.HomeBuyerNumber);

                homeOwner.HomeOwnerNumber = (ownerNumber == 0) ? null : (int?)ownerNumber;
                homeOwner.HomeOwnerName = (message.BuyerName.IsNullOrEmpty()) ? null : message.BuyerName;
                _database.Update(homeOwner);
            }
        }
    }
}