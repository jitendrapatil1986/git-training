using System;
using AutoMapper;
using Common.Messages;
using NServiceBus;
using NServiceBus.Saga;
using TIPS.Commands.Requests;
using TIPS.Commands.Responses;
using TIPS.Events.JobEvents;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;
using Warranty.Core.Services;
using Warranty.Server.Handlers.Jobs;

namespace Warranty.Server.Sagas
{
    public class BuyerTransferredToNewLotSaga : Saga<BuyerTransferredToNewLotSagaData>,
        IAmStartedByMessages<BuyerTransferredToNewLot>,
        IHandleMessages<BuyerTransferredToNewLotSaga_RemoveExistingHomeOwner>,
        IHandleMessages<BuyerTransferredToNewLotSaga_EnsureNewJobExists>,
        IHandleMessages<BuyerTransferredToNewLotSaga_AssignHomeownerAndTasks>,
        IHandleMessages<HomeBuyerDetailsResponse>,
        IHandleMessages<JobSaleDetailsResponse>
    {
        private readonly IJobService _jobService;
        private readonly IHomeOwnerService _homeOwnerService;
        private readonly ITaskService _taskService;
        private readonly IEmployeeService _employeeService;
        private readonly ICommunityService _communityService;
        private const string ENDPOINT_NAME = "Warranty.Server";

        public BuyerTransferredToNewLotSaga(IJobService jobService, IHomeOwnerService homeOwnerService, ITaskService taskService, IEmployeeService employeeService, ICommunityService communityService)
        {
            _jobService = jobService;
            _homeOwnerService = homeOwnerService;
            _taskService = taskService;
            _employeeService = employeeService;
            _communityService = communityService;
        }

        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping<BuyerTransferredToNewLotSaga_RemoveExistingHomeOwner>(x => x.NewJobNumber).ToSaga(x => x.NewJobNumber);
            ConfigureMapping<BuyerTransferredToNewLotSaga_EnsureNewJobExists>(x => x.NewJobNumber).ToSaga(x => x.NewJobNumber);
            ConfigureMapping<BuyerTransferredToNewLotSaga_AssignHomeownerAndTasks>(x => x.NewJobNumber).ToSaga(x => x.NewJobNumber);
            ConfigureMapping<HomeBuyerDetailsResponse>(x => x.ContactId).ToSaga(x => x.ContactId);
            ConfigureMapping<JobSaleDetailsResponse>(x => x.JobNumber).ToSaga(x => x.NewJobNumber);
        }

        public void Handle(BuyerTransferredToNewLot message)
        {
            Data.NewJobNumber = message.NewJobNumber;
            Data.PreviousJobNumber = message.PreviousJobNumber;
            Data.ContactId = message.ContactId;

            var homeOwner = _homeOwnerService.GetHomeOwnerByJobNumber(message.PreviousJobNumber);
            if (homeOwner == null)
            {
                // Get Homebuyer information from TIPS
                Bus.Send(new RequestHomeBuyerDetails(Data.ContactId));
                return;
            }

            Data.HomeOwner = homeOwner;
            Bus.SendLocal(new BuyerTransferredToNewLotSaga_RemoveExistingHomeOwner(Data.NewJobNumber));
        }

        public void Handle(HomeBuyerDetailsResponse message)
        {
            var homeOwner = Mapper.Map<HomeOwner>(message);
            homeOwner.HomeOwnerId = Guid.NewGuid();
            homeOwner.HomeOwnerNumber = 1;
            homeOwner.CreatedBy = ENDPOINT_NAME;
            homeOwner.UpdatedBy = ENDPOINT_NAME;
            homeOwner.CreatedDate = DateTime.UtcNow;
            homeOwner.UpdatedDate = DateTime.UtcNow;

            Data.HomeOwner = _homeOwnerService.Create(homeOwner);
            Bus.SendLocal(new BuyerTransferredToNewLotSaga_EnsureNewJobExists(Data.NewJobNumber));
        }

        public void Handle(BuyerTransferredToNewLotSaga_RemoveExistingHomeOwner message)
        {
            var homeOwner = _homeOwnerService.GetHomeOwnerByJobNumber(Data.PreviousJobNumber);
            var previousJob = _jobService.GetJobByNumber(Data.PreviousJobNumber);

            _homeOwnerService.RemoveFromJob(homeOwner, previousJob);

            _taskService.DeleteTask(previousJob.JobId, TaskType.JobStage3);
            _taskService.DeleteTask(previousJob.JobId, TaskType.JobStage10);
            _taskService.CreateTasks(previousJob.JobId);

            Bus.SendLocal(new BuyerTransferredToNewLotSaga_EnsureNewJobExists(Data.NewJobNumber));
        }

        public void Handle(BuyerTransferredToNewLotSaga_EnsureNewJobExists message)
        {
            var newJob = _jobService.GetJobByNumber(Data.NewJobNumber);

            if (newJob == null)
            {
                // Get Sale from TIPS
                Bus.Send(new RequestJobSaleDetails { JobNumber = Data.NewJobNumber });
                return;
            }

            Data.NewJob = newJob;
            Bus.SendLocal(new BuyerTransferredToNewLotSaga_AssignHomeownerAndTasks(Data.NewJobNumber));
        }

        public void Handle(JobSaleDetailsResponse message)
        {
            var newJob = Mapper.Map<Job>(message);

            var builder = _employeeService.GetEmployeeByNumber(message.BuilderEmployeeID);
            if (builder != null)
                newJob.BuilderEmployeeId = builder.EmployeeId;

            var community = _communityService.GetCommunityByNumber(message.CommunityNumber);
            if(community != null)
                newJob.CommunityId = community.CommunityId;

            Data.NewJob = _jobService.CreateJob(newJob);
            Bus.SendLocal(new BuyerTransferredToNewLotSaga_AssignHomeownerAndTasks(Data.NewJobNumber));
        }

        public void Handle(BuyerTransferredToNewLotSaga_AssignHomeownerAndTasks message)
        {
            _homeOwnerService.AssignToJob(Data.HomeOwner, Data.NewJob);
            _taskService.CreateTasks(Data.NewJob.JobId);

            MarkAsComplete();
        }
    }

    public class BuyerTransferredToNewLotSaga_AssignHomeownerAndTasks : IBusCommand
    {
        public string NewJobNumber { get; set; }

        public BuyerTransferredToNewLotSaga_AssignHomeownerAndTasks() { }

        public BuyerTransferredToNewLotSaga_AssignHomeownerAndTasks(string newJobNumber)
        {
            NewJobNumber = newJobNumber;
        }
    }

    public class BuyerTransferredToNewLotSaga_EnsureNewJobExists : IBusCommand
    {
        public string NewJobNumber { get; set; }

        public BuyerTransferredToNewLotSaga_EnsureNewJobExists() { }

        public BuyerTransferredToNewLotSaga_EnsureNewJobExists(string newJobNumber)
        {
            NewJobNumber = newJobNumber;
        }
    }

    public class BuyerTransferredToNewLotSaga_RemoveExistingHomeOwner : IBusCommand
    {
        public string NewJobNumber { get; set; }

        public BuyerTransferredToNewLotSaga_RemoveExistingHomeOwner() { }

        public BuyerTransferredToNewLotSaga_RemoveExistingHomeOwner(string newJobNumber)
        {
            NewJobNumber = newJobNumber;
        }
    }

    public class BuyerTransferredToNewLotSagaData : IContainSagaData
    {
        public Guid Id { get; set; }
        public string Originator { get; set; }
        public string OriginalMessageId { get; set; }

        public string NewJobNumber { get; set; }
        public string PreviousJobNumber { get; set; }

        [Unique]
        public Guid ContactId { get; set; }
        public HomeOwner HomeOwner { get; set; }
        public Job NewJob { get; set; }
    }
}