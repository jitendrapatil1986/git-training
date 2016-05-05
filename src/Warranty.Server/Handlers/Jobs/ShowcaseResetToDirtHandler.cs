namespace Warranty.Server.Handlers.Jobs
{
    using System;
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

        public ShowcaseResetToDirtHandler(IDatabase database, ILog log)
        {
            _database = database;
            _log = log;
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
                _log.ErrorFormat("Job {0} received a showcase reset to dirt message but has a homeowner.", job.JobNumber);
                throw new ArgumentException(string.Format("Job {0} has a homeowner and can not be reset to dirt", job.JobNumber));
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