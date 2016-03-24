using System;
using System.Linq;
using log4net;
using NPoco;
using NServiceBus;
using Warranty.Core.DataAccess;
using Warranty.Core.Entities;
using TIPS.Events.JobEvents;
using Warranty.Core.Enumerations;
using Warranty.Core.Services;

namespace Warranty.Server.Handlers.Jobs
{
    public class HomeSaleCancelledHandler : IHandleMessages<HomeSaleCancelled>
    {
        private readonly SqlServerDatabase _database;
        private readonly IHomeOwnerService _homeOwnerService;
        private readonly ITaskService _taskService;
        private readonly ILog _log;

        public HomeSaleCancelledHandler(IDatabase database, IHomeOwnerService homeOwnerService, ITaskService taskService, ILog log)
        {
            _database = (SqlServerDatabase) database;
            _homeOwnerService = homeOwnerService;
            _taskService = taskService;
            _log = log;
        }

        public void Handle(HomeSaleCancelled message)
        {
            using (_database)
            {
                var job = _database.FetchWhere<Job>(j => j.JobNumber == message.JobNumber).SingleOrDefault();
                if (job == null)
                {
                    _log.WarnFormat("Received HomeSaleCancelled for job {0} that does not exist locally", message.JobNumber);
                    return;
                }

                var homeOwner = _homeOwnerService.GetHomeOwnerByJobNumber(message.JobNumber);

                if (homeOwner != null)
                {
                    _log.Info(string.Format(@"Deleting HomeOwner: Name {0}, Number {1}, Phone {2}, Job Number {3}", homeOwner.HomeOwnerName, homeOwner.HomeOwnerNumber, homeOwner.HomePhone, job.JobNumber));
                    job.CurrentHomeOwnerId = null;
                    _database.Update(job);
                    _database.Delete(homeOwner);
                    RemoveToDos(job.JobId);
                }
                else
                {
                    _log.Error(string.Format("Deleting HomeOwner Failed: Homeowner does not exist for job {0}", job.JobNumber));
                }
            }
        }

        private void RemoveToDos(Guid jobId)
        {
            _taskService.DeleteTask(jobId, TaskType.JobStage3);
            _taskService.DeleteTask(jobId, TaskType.JobStage10);
        }
    }
}