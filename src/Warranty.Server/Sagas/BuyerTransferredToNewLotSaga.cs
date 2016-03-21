using System;
using AutoMapper;
using Common.Messages;
using log4net;
using Newtonsoft.Json;
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
        IHandleMessages<BuyerTransferredToNewLotSaga_AssignHomeownerAndTasks>,
        IHandleMessages<HomeBuyerDetailsResponse>,
        IHandleMessages<JobSaleDetailsResponse>
    {
        private readonly IJobService _jobService;
        private readonly IHomeOwnerService _homeOwnerService;
        private readonly ITaskService _taskService;
        private readonly IEmployeeService _employeeService;
        private readonly ICommunityService _communityService;
        private readonly ILog _log;
        private const string ENDPOINT_NAME = "Warranty.Server";

        public BuyerTransferredToNewLotSaga(IJobService jobService, IHomeOwnerService homeOwnerService, ITaskService taskService, IEmployeeService employeeService, ICommunityService communityService, ILog log)
        {
            _jobService = jobService;
            _homeOwnerService = homeOwnerService;
            _taskService = taskService;
            _employeeService = employeeService;
            _communityService = communityService;
            _log = log;
        }

        public BuyerTransferredToNewLotSaga() { }

        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping<BuyerTransferredToNewLotSaga_RemoveExistingHomeOwner>(x => x.NewJobNumber).ToSaga(x => x.NewJobNumber);
            ConfigureMapping<BuyerTransferredToNewLotSaga_AssignHomeownerAndTasks>(x => x.NewJobNumber).ToSaga(x => x.NewJobNumber);
            ConfigureMapping<HomeBuyerDetailsResponse>(x => x.ContactId).ToSaga(x => x.ContactId);
            ConfigureMapping<JobSaleDetailsResponse>(x => x.JobNumber).ToSaga(x => x.NewJobNumber);
        }

        public void Handle(BuyerTransferredToNewLot message)
        {
            _log.InfoFormat("Received BuyerTransferredToNewLot for PreviousJob {0} to NewJob {1} for ContactId {2}", message.PreviousJobNumber, message.NewJobNumber, message.ContactId);
            Data.NewJobNumber = message.NewJobNumber;
            Data.PreviousJobNumber = message.PreviousJobNumber;
            Data.ContactId = message.ContactId;

            var homeOwner = _homeOwnerService.GetHomeOwnerByJobNumber(message.PreviousJobNumber);
            if (homeOwner == null)
            {
                _log.InfoFormat("HomeOwner was not found in Warranty for job {0}, requesting HomeBuyer details from TIPS", message.PreviousJobNumber);
                // Get Homebuyer information from TIPS
                Bus.Send(new RequestHomeBuyerDetails(Data.ContactId));
                return;
            }

            _log.InfoFormat("Existing HomeOwner found in Warranty for job {0}, proceeding to remove them from the job.", message.PreviousJobNumber);
            Data.HomeOwner = homeOwner;
            Bus.SendLocal(new BuyerTransferredToNewLotSaga_RemoveExistingHomeOwner(Data.NewJobNumber));
        }

        public void Handle(HomeBuyerDetailsResponse message)
        {
            _log.InfoFormat("Response for HomeBuyerDetails received from TIPS for ContactId {0} - {1}", message.ContactId, JsonConvert.SerializeObject(message));

            var homeOwner = Mapper.Map<HomeOwner>(message);
            homeOwner.HomeOwnerId = Guid.NewGuid();
            homeOwner.HomeOwnerNumber = 1;
            homeOwner.CreatedBy = ENDPOINT_NAME;
            homeOwner.UpdatedBy = ENDPOINT_NAME;
            homeOwner.CreatedDate = DateTime.UtcNow;
            homeOwner.UpdatedDate = DateTime.UtcNow;

            _log.InfoFormat("Creating new HomeOwner in Warranty from TIPS information = {0}", JsonConvert.SerializeObject(homeOwner));
            Data.HomeOwner = _homeOwnerService.Create(homeOwner);

            _log.InfoFormat("Requesting updated job details from TIPS {0}", Data.NewJobNumber);
            Bus.Send(new RequestJobSaleDetails { JobNumber = Data.NewJobNumber });
        }

        public void Handle(BuyerTransferredToNewLotSaga_RemoveExistingHomeOwner message)
        {
            _log.InfoFormat("Removing HomeOwner from previous job {0}", Data.PreviousJobNumber);

            var homeOwner = _homeOwnerService.GetHomeOwnerByJobNumber(Data.PreviousJobNumber);
            var previousJob = _jobService.GetJobByNumber(Data.PreviousJobNumber);

            _homeOwnerService.RemoveFromJob(homeOwner, previousJob);

            _log.InfoFormat("Removing tasks from previous job {0}", Data.PreviousJobNumber);
            _taskService.DeleteTask(previousJob.JobId, TaskType.JobStage3);
            _taskService.DeleteTask(previousJob.JobId, TaskType.JobStage10);

            _log.InfoFormat("Creating tasks from previous job {0}", Data.PreviousJobNumber);
            _taskService.CreateTasks(previousJob.JobId);

            _log.InfoFormat("Requesting updated job details from TIPS {0}", Data.NewJobNumber);
            Bus.Send(new RequestJobSaleDetails { JobNumber = Data.NewJobNumber });
        }

        public void Handle(JobSaleDetailsResponse message)
        {
            _log.InfoFormat("Received JobDetailsResponse from TIPS for job number {0}, will now create job in Warranty.", message.JobNumber);

            var existingJob = _jobService.GetJobByNumber(message.JobNumber);

            if (existingJob == null)
            {
                _log.InfoFormat("Job {0} does not exist, will create a new job with details from TIPS.", message.JobNumber);
                Data.NewJob = Mapper.Map<Job>(message);
                Data.NewJob.CreatedBy = ENDPOINT_NAME;
                Data.NewJob.UpdatedBy = ENDPOINT_NAME;
                Data.NewJob.CreatedDate = DateTime.UtcNow;
                Data.NewJob.UpdatedDate = DateTime.UtcNow;
            }
            else
            {
                _log.InfoFormat("Job {0} exists, will update the existing job with details from TIPS.", message.JobNumber);
                Data.NewJob = Mapper.Map(message, existingJob);
                Data.NewJob.UpdatedBy = ENDPOINT_NAME;
                Data.NewJob.UpdatedDate = DateTime.UtcNow;
            }
            

            var builder = _employeeService.GetEmployeeByNumber(message.BuilderEmployeeID);
            if (builder != null)
            {
                _log.InfoFormat("Found a builder from the JobDetailsResponse for employeeid {0}, assigning it to the job.", message.BuilderEmployeeID);
                Data.NewJob.BuilderEmployeeId = builder.EmployeeId;
            }

            var community = _communityService.GetCommunityByNumber(message.CommunityNumber);
            if (community != null)
            {
                _log.InfoFormat("Found a community from the JobDetailsResponse for CommunityNumber {0}, assigning it to the job.", message.CommunityNumber);
                Data.NewJob.CommunityId = community.CommunityId;
            }

            if (Data.NewJob.IsNew())
            {
                _log.InfoFormat("Saving new job record in Warranty for job number {0}.", message.JobNumber);
                Data.NewJob = _jobService.CreateJob(Data.NewJob);
            }
            else
            {
                _log.InfoFormat("Updating job {0} in Warranty with new details from TIPS.", message.JobNumber);
                _jobService.Save(Data.NewJob);
            }
                

            _log.InfoFormat("Proceeding to assign the HomeOwner and create tasks for Job {0}.", Data.NewJob.JobNumber);
            Bus.SendLocal(new BuyerTransferredToNewLotSaga_AssignHomeownerAndTasks(Data.NewJobNumber));
        }

        public void Handle(BuyerTransferredToNewLotSaga_AssignHomeownerAndTasks message)
        {
            _homeOwnerService.AssignToJob(Data.HomeOwner, Data.NewJob);
            _log.InfoFormat("Assigned HomeOwner {0} to JobNumber {1}.", Data.HomeOwner.HomeOwnerNumber, Data.NewJob.JobNumber);

            _taskService.CreateTasks(Data.NewJob.JobId);
            _log.InfoFormat("Created tasks for JobNumber {0}.", Data.NewJob.JobNumber);

            MarkAsComplete();
            _log.InfoFormat("BuyerTransferredToNewLogSaga complete for JobNumber {0}.", Data.NewJob.JobNumber);
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