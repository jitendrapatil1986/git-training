using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.DateFormatter;
using NHibernate.Mapping.ByCode.Impl;
using NHibernate.Util;
using NPoco;
using NServiceBus;
using Warranty.Core.DataAccess;
using Warranty.Core.Entities;
using TIPS.Events.JobEvents;
using TIPS.Events.Models;
using Warranty.Core.ActivityLogger;
using Warranty.Server.Extensions;
using Job = Warranty.Core.Entities.Job;

namespace Warranty.Server.Handlers.Jobs
{
    public class JobSaleApprovedHandler : IHandleMessages<JobSaleApproved>
    {
        private readonly IDatabase _database;
        private static readonly ILog _log = LogManager.GetLogger(typeof(JobSaleApprovedHandler));

        public JobSaleApprovedHandler(IDatabase database, IActivityLogger activityLogger)
        {
            _database = database;
        }

        private void SetIfNotNull<T>(List<T> list, Func<T,bool> condition, Action<T> setter)
        {
            var check = list.FirstOrDefault(condition);
            if (check != null)
            {
                setter(check);
            }
        }

        public HomeOwner GetHomeOwner(JobSaleApproved message, Job job)
        {
            
            var homeOwner = new HomeOwner
            {
                HomeOwnerId = Guid.NewGuid(),
                CreatedBy = "Warranty.Server",
                CreatedDate = DateTime.UtcNow,
                HomeOwnerNumber = 0,
                JobId = job.JobId,
                UpdatedBy = "Warranty.Server"
            };

            var homeOwnerInfo = message.Opportunity.Contact;
            if (homeOwnerInfo != null && homeOwnerInfo.FirstName != null && homeOwnerInfo.LastName != null)
                homeOwner.HomeOwnerName = string.Format("{0}, {1}", homeOwnerInfo.LastName,
                    homeOwnerInfo.FirstName);

            job.CurrentHomeOwnerId = homeOwner.HomeOwnerId;

            if (message.Opportunity.Contact != null)
            {
                SetIfNotNull(message.Opportunity.Contact.PhoneNumbers, x => x.IsPrimary, x => homeOwner.HomePhone = x.Number);
                SetIfNotNull(message.Opportunity.Contact.Emails, x => x.IsPrimary, x => homeOwner.EmailAddress = x.Address);
            }

            return homeOwner;
        }

        public Job GetJob(JobSaleApproved message, Guid? communityId = null, Guid? builderEmployeeId = null, Guid? salesEmployeeId = null)
        {
            var job = new Job
            {
                JobId = Guid.NewGuid(),
                JobNumber = message.Sale.JobNumber,
                PlanNumber = message.Sale.PlanNumber,
                Elevation = message.Sale.Elevation,
                AddressLine = message.Sale.AddressLine1,
                City = message.Sale.AddressCity,
                StateCode = message.Sale.AddressStateAbbreviation,
                PostalCode = message.Sale.AddressZipCode,
                PlanType = message.Sale.JobType,
                CloseDate = message.Sale.CloseDate,
                CreatedBy = "Warranty.Server",
                CreatedDate = DateTime.Now,
                JdeIdentifier = null, //TIPS needs to start generating these JDE Identifiers for me
                PlanName = message.Sale.PlanName,
                PlanTypeDescription = null, //TIPS should tell me what kind of home this is
                Swing = message.Sale.Swing,
            };

            if (message.Sale.LegalDescription != null)
                job.LegalDescription = message.Sale.LegalDescription.ToString();
            if (communityId.HasValue) 
                job.CommunityId = communityId.Value;
            if (builderEmployeeId.HasValue) 
                job.BuilderEmployeeId = builderEmployeeId.Value;
            if (salesEmployeeId.HasValue) 
                job.SalesConsultantEmployeeId = salesEmployeeId.Value;
            if (message.Sale.Stage.HasValue)
                job.Stage = message.Sale.Stage.Value;
            if (message.Sale.CloseDate.HasValue)
                job.WarrantyExpirationDate = message.Sale.CloseDate.Value.AddYears(10);

            return job;
        }

        public void Validate(JobSaleApproved message)
        {
            StringBuilder sb = new StringBuilder();

            if (String.IsNullOrWhiteSpace(message.Sale.JobNumber))
            {
                sb.Append("Job number missing. ");
            }
            if (String.IsNullOrWhiteSpace(message.Sale.CommunityNumber))
            {
                sb.Append("Community number missing. ");
            }
            
            if (sb.Length > 0)
            {
                throw new ArgumentException(sb.ToString());
            }
        }

        public void DeletePreviousHomeowners(Job job)
        {
            _log.Info(string.Format(@"Deleting previous homeowners for job {0}", job.JobNumber));
            using (_database)
            {
                var homeOwnersForJob = _database.GetHomeOwnersByJobNumber(job.JobNumber);
                if (homeOwnersForJob.Count > 0)
                {
                    job.CurrentHomeOwnerId = null;
                    _database.Update(job);
                    foreach (var previousHomeOwner in homeOwnersForJob)
                    {
                        _database.Delete(previousHomeOwner);
                    }
                } 
            }
        }

        public void Handle(JobSaleApproved message)
        {
            Validate(message);

            using (_database)
            {
                var community = _database.GetCommunityByNumber(message.Sale.CommunityNumber);
                if (community == null) throw new ArgumentException(string.Format("Community number '{0}' does not exist in database", message.Sale.CommunityNumber));

                var job = _database.GetJobByNumber(message.Sale.JobNumber);
                var builder = _database.GetEmployeeByNumber(message.Sale.BuilderEmployeeID);
                var salesConsultant = _database.GetEmployeeByNumber(message.Sale.SalesConsultantEmployeeID);

                if (job == null)
                {
                    _log.Info(string.Format(@"Creating Job: {0}", message.Sale.JobNumber));
                    job = GetJob(message);
                    job.CommunityId = community.CommunityId;
                    _database.Insert(job);
                }
                DeletePreviousHomeowners(job);

                var homeOwner = GetHomeOwner(message, job);
                _database.Insert(homeOwner);
                
                if (builder != null) job.BuilderEmployeeId = builder.EmployeeId;
                if (salesConsultant != null) job.SalesConsultantEmployeeId = salesConsultant.EmployeeId;
                if (community.CommunityId != job.CommunityId) job.CommunityId = community.CommunityId;
                
                job.CurrentHomeOwnerId = homeOwner.HomeOwnerId;

                _database.Update(job);
            }
        }
    }
}
