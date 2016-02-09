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
        private readonly ILog _log = LogManager.GetLogger(typeof(ShowcaseResetToDirtHandler));

        public ShowcaseResetToDirtHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(ShowcaseResetToDirt message)
        {
            var job = _database.SingleOrDefaultByJdeId<Job>(message.JobNumber);

            if (job == null)
            {
                _log.Info(string.Format("Showcase {0} was reset to dirt but not found.", message.JobNumber));
            }
            else
            {
                var tasks = _database.Fetch<Task>("where ReferenceId = @0", job.JobId);
                foreach (var task in tasks)
                {
                    _log.Info(string.Format("Deleting task {0} because job {1} was a showcase that was reset to dirt", task.TaskId, job.JobNumber));
                    _database.Delete(task);
                }

                _log.Info(string.Format("Deleting job {0} as it is a showcase reset to dirt.", job.JobNumber));
                _database.Delete(job);
            }
        }
    }
}