using System;
using System.Collections.Generic;
using System.Linq;
using NPoco;
using TIPS.Events.Models;
using Warranty.Core.Entities;
using Warranty.Server.Handlers.Jobs;
using Job = Warranty.Core.Entities.Job;

namespace Warranty.Core.Services
{
    public class HomeOwnerService : IHomeOwnerService
    {
        private readonly IDatabase _database;

        public HomeOwnerService(IDatabase database)
        {
            _database = database;
        }

        public void AssignToJob(HomeOwner homeOwner, Job job)
        {
            job.CurrentHomeOwnerId = homeOwner.HomeOwnerId;
            homeOwner.JobId = job.JobId;
        }

        public HomeOwner GetHomeOwnerByJobNumber(string jobNumber)
        {
            return _database.SingleOrDefault<HomeOwner>(
                @"SELECT TOP 1 
                       h.HomeownerId
                      ,h.JobId
                      ,h.HomeownerNumber
                      ,h.HomeownerName
                      ,h.HomePhone
                      ,h.OtherPhone
                      ,h.WorkPhone1
                      ,h.WorkPhone2
                      ,h.EmailAddress
                      ,h.CreatedDate
                      ,h.CreatedBy
                      ,h.UpdatedDate
                      ,h.UpdatedBy
                      ,h.OldHomeOwnerID 
                FROM Homeowners h 
                inner join Jobs j on j.JobId = h.JobId 
                WHERE j.JobNumber = @0 
                ORDER BY h.HomeownerNumber DESC", jobNumber);
        }

        public HomeOwner GetHomeOwner(Contact homeOwnerInfo, int homeOwnerNumber = 1)
        {
            var homeOwner = new HomeOwner
            {
                HomeOwnerId = Guid.NewGuid(),
                CreatedBy = "Warranty.Server",
                CreatedDate = DateTime.UtcNow,
                HomeOwnerNumber = homeOwnerNumber,
                UpdatedBy = "Warranty.Server"
            };

            if (homeOwnerInfo != null)
            {
                if (homeOwnerInfo.FirstName != null && homeOwnerInfo.LastName != null)
                {
                    homeOwner.HomeOwnerName = string.Format("{0}, {1}", homeOwnerInfo.LastName,
                        homeOwnerInfo.FirstName);
                }

                if (homeOwnerInfo.PhoneNumbers != null)
                {
                    var primaryPhone = homeOwnerInfo.PhoneNumbers.Where(x => x.IsPrimary).ToList();
                    if (primaryPhone.Count == 1)
                    {
                        homeOwner.HomePhone = primaryPhone[0].Number;
                    }
                }
                if (homeOwnerInfo.Emails != null)
                {
                    var primaryEmail = homeOwnerInfo.Emails.Where(x => x.IsPrimary).ToList();
                    if (primaryEmail.Count == 1)
                    {
                        homeOwner.EmailAddress = primaryEmail[0].Address;
                    }
                }
            }

            return homeOwner;
        }
    }
}