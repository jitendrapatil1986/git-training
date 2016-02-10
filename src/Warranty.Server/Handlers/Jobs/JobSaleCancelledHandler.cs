using System.Linq;
using log4net;
using NPoco;
using NServiceBus;
using Warranty.Core.DataAccess;
using Warranty.Core.Entities;
using TIPS.Events.JobEvents;
using Warranty.Server.Extensions;

namespace Warranty.Server.Handlers.Jobs
{
    public class JobSaleCancelledHandler : IHandleMessages<JobSaleCancelled>
    {
        private readonly SqlServerDatabase _database;
        private readonly ILog _log = LogManager.GetLogger(typeof (JobSaleCancelledHandler));
        private readonly IHomeOwnerService _homeOwnerService;

        public JobSaleCancelledHandler(IDatabase database, IHomeOwnerService homeOwnerService)
        {
            _database = (SqlServerDatabase) database;
            _homeOwnerService = homeOwnerService;
        }

        public void Handle(JobSaleCancelled message)
        {
            using (_database)
            {
                var job = _database.FetchWhere<Job>(j => j.JobNumber == message.JobNumber).Single();
                var homeOwner = _homeOwnerService.GetHomeOwnerByJobNumber(message.JobNumber);

                if (homeOwner != null)
                {
                    _log.Info(string.Format(@"Deleting HomeOwner: Name {0}, Number {1}, Phone {2}, Job Number {3}",
                        homeOwner.HomeOwnerName, homeOwner.HomeOwnerNumber, homeOwner.HomePhone, job.JobNumber));
                    job.CurrentHomeOwnerId = null;
                    _database.Update(job);
                    _database.Delete(homeOwner);
                }
                else
                {
                    _log.Error(string.Format("Deleting HomeOwner Failed: Homeowner does not exist for job {0}",
                        job.JobNumber));
                }
            }
        }
    }
}