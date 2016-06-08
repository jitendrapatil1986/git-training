using System.Linq;
using NHibernate.Util;

namespace Warranty.Server.Handlers.Jobs
{
    using Core.Entities;
    using Extensions;
    using log4net;
    using NPoco;
    using NServiceBus;
    using TIPS.Events.JobEvents;
    public class ShowcaseResetToDirtHandler : IHandleMessages<ShowcaseResetToDirt>
    {
        private readonly IDatabase _database;
        private readonly ILog _log;
        private readonly IHomeOwnerService _homeOwnerService;

        public ShowcaseResetToDirtHandler(IDatabase database, ILog log, IHomeOwnerService homeOwnerService)
        {
            _database = database;
            _log = log;
            _homeOwnerService = homeOwnerService;
        }

        public void Handle(ShowcaseResetToDirt message)
        {
            _log.InfoFormat("Received ShowcaseResetToDirt for JobNumber {0}", message.JobNumber);
            var job = _database.SingleOrDefaultByJdeId<Job>(message.JobNumber);

            if (job == null)
            {
                _log.InfoFormat("Showcase {0} was reset to dirt but not found.", message.JobNumber);
                return;
            }

            if (job.CurrentHomeOwnerId != null)
            {
                _log.ErrorFormat("Job {0} received a showcase reset to dirt message but has homeowner {1} currently assigned. Proceeding with deletion.", 
                    message.JobNumber, 
                    job.CurrentHomeOwnerId);
            }

            _log.ErrorFormat("Deleting HomeOwner {0} for JobNumber {1} ({2})", job.CurrentHomeOwnerId, message.JobNumber, job.JobId);
            _homeOwnerService.RemoveHomeOwner(job);

            var otherHomeOwnersAssignedToJob = _database.Fetch<HomeOwner>("WHERE JobId = @0;", job.JobId);

            if (otherHomeOwnersAssignedToJob.Any())
            {
                var homeOwnerGuids = string.Join(",", otherHomeOwnersAssignedToJob.Select(x => x.HomeOwnerId.ToString()));
                _log.ErrorFormat("Deleting additional home owners found ({0}) for JobNumber {1} ({2})", homeOwnerGuids, message.JobNumber, job.JobId);
                _database.Delete<HomeOwner>("WHERE JobID = @0;", job.JobId);
            }

            _log.InfoFormat("Deleting Tasks for JobNumber {0} ({1})", message.JobNumber, job.JobId);
            _database.Delete<Task>("WHERE ReferenceId = @0", job.JobId);
            
            _log.InfoFormat("Deleting JobStages for JobNumber {0} ({1})", message.JobNumber, job.JobId);
            _database.Delete<JobStage>("WHERE JobId = @0", job.JobId);

            _log.InfoFormat("Deleting Job with JobNumber {0} ({1})", message.JobNumber, job.JobId);
            _database.Delete(job);
        }
    }
}