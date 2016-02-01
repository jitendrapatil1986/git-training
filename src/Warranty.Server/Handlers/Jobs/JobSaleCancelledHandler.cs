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
        private static readonly ILog _log = LogManager.GetLogger(typeof (JobSaleCancelledHandler));

        public JobSaleCancelledHandler(IDatabase database)
        {
            _database = (SqlServerDatabase) database;
        }

        public void Handle(JobSaleCancelled message)
        {
            using (_database)
            {
                var job = _database.FetchWhere<Job>(j => j.JobNumber == message.JobNumber).Single();
                var homeOwners = _database.GetHomeOwnersByJobNumber(message.JobNumber);

                if (homeOwners != null)
                {
                    foreach (var homeOwner in homeOwners)
                    {
                        _log.Info(string.Format(@"Deleting HomeOwner: Name {0}, Number {1}, Phone {2}, Job Number {3}",
                            homeOwner.HomeOwnerName, homeOwner.HomeOwnerNumber, homeOwner.HomePhone, job.JobNumber));
                        _database.Delete(homeOwner);
                    }
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