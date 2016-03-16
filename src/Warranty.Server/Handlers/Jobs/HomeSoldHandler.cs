using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using NPoco;
using NServiceBus;
using TIPS.Events.JobEvents;
using Warranty.Core.Enumerations;
using Warranty.Core.Services;
using Job = Warranty.Core.Entities.Job;

namespace Warranty.Server.Handlers.Jobs
{
    public class HomeSoldHandler : IHandleMessages<HomeSold>
    {
        private readonly IDatabase _database;
        private readonly ILog _log = LogManager.GetLogger(typeof (HomeSoldHandler));
        private readonly IJobService _jobService;
        private readonly IHomeOwnerService _homeOwnerService;
        private readonly ICommunityService _communityService;
        private readonly ITaskService _taskService;

        public HomeSoldHandler(IDatabase database, IJobService jobService, IHomeOwnerService homeOwnerService, ICommunityService communityService, ITaskService taskService)
        {
            _database = database;
            _jobService = jobService;
            _homeOwnerService = homeOwnerService;
            _communityService = communityService;
            _taskService = taskService;
        }

        public void Validate(HomeSold message)
        {
            //todo: Validate
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

        public void Handle(HomeSold message)
        {
            Validate(message);

            //using (_database)
            //{
            //    var community = _communityService.GetCommunityByNumber(message.Sale.CommunityNumber);
            //    if (community == null)
            //    {
            //        throw new ArgumentException(string.Format("Community number '{0}' does not exist in database",
            //            message.Sale.CommunityNumber));
            //    }

            //    var job = _jobService.GetJobByNumber(message.Sale.JobNumber);

            //    if (job == null)
            //    {
            //        _log.InfoFormat(@"Creating Job: {0}", message.Sale.JobNumber);
            //        job = _jobService.CreateJob(message.Sale);
            //    }
            //    else
            //    {
            //        _log.InfoFormat(@"Updating Job: {0}", message.Sale.JobNumber);
            //        _jobService.UpdateExistingJob(job, message.Sale);
            //    }

            //    //todo: Once the information is correct in Warranty DB, existing homeowner will be an error
            //    DeletePreviousHomeowners(job);
                
            //    var homeOwner = _homeOwnerService.GetHomeOwner(message.Opportunity.Contact);

            //    _database.Insert(homeOwner);
            //    _homeOwnerService.AssignToJob(homeOwner, job);
            //    _database.Update(job);

            //    _taskService.CreateTasks(job.JobId);
            //}
        }
    }
}