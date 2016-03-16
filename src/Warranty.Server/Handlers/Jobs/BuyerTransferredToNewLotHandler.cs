using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPoco;
using NServiceBus;
using TIPS.Events.JobEvents;
using TIPS.Events.Models;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;
using Warranty.Core.Services;
using Job = Warranty.Core.Entities.Job;

namespace Warranty.Server.Handlers.Jobs
{
    public class BuyerTransferredToNewLotHandler : IHandleMessages<BuyerTransferredToNewLot>
    {
        private readonly IJobService _jobService;
        private readonly IHomeOwnerService _homeOwnerService;
        private readonly ITaskService _taskService;
        private IDatabase _database;

        public BuyerTransferredToNewLotHandler(IJobService jobService, IHomeOwnerService homeOwnerService, ITaskService taskService, IDatabase database)
        {
            _jobService = jobService;
            _homeOwnerService = homeOwnerService;
            _taskService = taskService;
            _database = database;
        }

        private void Validate(BuyerTransferredToNewLot message)
        {
            
        }

        private HomeOwner CreateNewHomeowner(Opportunity opportunity )
        {
            if (opportunity == null || opportunity.Contact == null)
                        throw new InvalidOperationException(
                            "Homeowner doesn't exist on previous job number, and the contact information doesn't exist to create a new homeowner.");

            return _homeOwnerService.GetHomeOwner(opportunity.Contact);
        }

        private void RemoveToDos(Job job)
        {
            _taskService.DeleteTask(job.JobId, TaskType.JobStage3);
            _taskService.DeleteTask(job.JobId, TaskType.JobStage10);
        }

        public void Handle(BuyerTransferredToNewLot message)
        {
            Validate(message);

            var previousJob = _jobService.GetJobByNumber(message.PreviousJobNumber);
            var newJob = _jobService.GetJobByNumber(message.NewJobNumber);
            var homeowner = _homeOwnerService.GetHomeOwnerByJobNumber(message.PreviousJobNumber);

            if (homeowner == null)
            {
                // todo: create homeowner
                //homeowner = CreateNewHomeowner(message.Opportunity);
            }
            else
            {
                _homeOwnerService.RemoveFromJob(homeowner, previousJob);
            }

            RemoveToDos(previousJob);
            _taskService.CreateTasks(previousJob.JobId);

            if (newJob == null)
            {
                //todo: create job
                //newJob = _jobService.CreateJob(message.Sale);
            }

            _homeOwnerService.AssignToJob(homeowner, newJob);
            _taskService.CreateTasks(newJob.JobId);
        }
    }
}
