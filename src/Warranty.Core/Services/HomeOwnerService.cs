using System;
using System.Collections.Generic;
using System.Linq;
using NPoco;
using TIPS.Events.JobEvents;
using TIPS.Events.Models;
using Warranty.Core.Entities;
using Job = Warranty.Core.Entities.Job;

namespace Warranty.Server.Handlers.Jobs
{
    public class HomeOwnerService : IHomeOwnerService
    {
        private readonly IDatabase _database;

        public HomeOwnerService(IDatabase database)
        {
            _database = database;
        }

        private void SetIfNotNull<T>(List<T> list, Func<T, bool> condition, Action<T> setter)
        {
            if (list == null) return;
            if (condition == null) return;

            var check = list.FirstOrDefault(condition);
            if (check != null)
            {
                setter(check);
            }
        }

        public void AssignToJob(HomeOwner homeOwner, Job job)
        {
            job.CurrentHomeOwnerId = homeOwner.HomeOwnerId;
            homeOwner.JobId = job.JobId;
        }

        public HomeOwner GetHomeOwnerByJobNumber(string jobNumber)
        {
            return _database.SingleOrDefault<HomeOwner>("SELECT h.* FROM Homeowners h inner join Jobs j on j.JobId = h.JobId WHERE j.JobNumber = @0", jobNumber);
        }

        public HomeOwner GetHomeOwner(Opportunity opportunity)
        {
            var homeOwner = new HomeOwner
            {
                HomeOwnerId = Guid.NewGuid(),
                CreatedBy = "Warranty.Server",
                CreatedDate = DateTime.UtcNow,
                HomeOwnerNumber = 0,
                UpdatedBy = "Warranty.Server"
            };

            var homeOwnerInfo = opportunity.Contact;
            if (homeOwnerInfo != null && homeOwnerInfo.FirstName != null && homeOwnerInfo.LastName != null)
            {
                homeOwner.HomeOwnerName = string.Format("{0}, {1}", homeOwnerInfo.LastName,
                    homeOwnerInfo.FirstName);
            }

            if (opportunity.Contact != null)
            {
                SetIfNotNull(opportunity.Contact.PhoneNumbers, x => x.IsPrimary,
                    x => homeOwner.HomePhone = x.Number);
                SetIfNotNull(opportunity.Contact.Emails, x => x.IsPrimary,
                    x => homeOwner.EmailAddress = x.Address);
            }

            return homeOwner;
        }
    }
}