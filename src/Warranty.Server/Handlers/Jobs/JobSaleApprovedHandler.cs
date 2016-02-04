using System;
using System.Text;
using log4net;
using NPoco;
using NServiceBus;
using TIPS.Events.JobEvents;
using Warranty.Core.Services;
using Job = Warranty.Core.Entities.Job;

namespace Warranty.Server.Handlers.Jobs
{
    public class JobSaleApprovedHandler : IHandleMessages<JobSaleApproved>
    {
        private readonly IDatabase _database;
        private static readonly ILog _log = LogManager.GetLogger(typeof (JobSaleApprovedHandler));
        private readonly IJobService _jobService;
        private readonly IHomeOwnerService _homeOwnerService;
        private ICommunityService _communityService;

        public JobSaleApprovedHandler(IDatabase database, IJobService jobService, IHomeOwnerService homeOwnerService, ICommunityService communityService)
        {
            _database = database;
            _jobService = jobService;
            _homeOwnerService = homeOwnerService;
            _communityService = communityService;
        }

        public void Validate(JobSaleApproved message)
        {
            var sb = new StringBuilder();

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
                var previousHomeOwner = _homeOwnerService.GetHomeOwnerByJobNumber(job.JobNumber);
                if (previousHomeOwner != null)
                {
                    job.CurrentHomeOwnerId = null;
                    _database.Update(job);
                    _database.Delete(previousHomeOwner);
                }
            }
        }

        public void Handle(JobSaleApproved message)
        {
            Validate(message);

            using (_database)
            {
                var community = _communityService.GetCommunityByNumber(message.Sale.CommunityNumber);
                if (community == null)
                {
                    throw new ArgumentException(string.Format("Community number '{0}' does not exist in database",
                        message.Sale.CommunityNumber));
                }

                var job = _jobService.GetJobByNumber(message.Sale.JobNumber);

                if (job == null)
                {
                    _log.Info(string.Format(@"Creating Job: {0}", message.Sale.JobNumber));
                    job = _jobService.CreateJobAndInsert(message.Sale);
                }
                DeletePreviousHomeowners(job);

                var homeOwner = _homeOwnerService.GetHomeOwner(message.Opportunity);
                _homeOwnerService.AssignToJob(homeOwner, job);

                _database.Insert(homeOwner);
                _database.Update(job);
            }
        }
    }
}