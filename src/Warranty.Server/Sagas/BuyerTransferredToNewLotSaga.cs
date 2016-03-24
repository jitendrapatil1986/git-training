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
        IHandleMessages<HomeBuyerDetailsResponse>,
        IHandleMessages<JobSaleDetailsResponse>
    {
        private readonly IJobService _jobService;
        private readonly IHomeOwnerService _homeOwnerService;
        private readonly ITaskService _taskService;
        private readonly IEmployeeService _employeeService;
        private readonly ICommunityService _communityService;
        private readonly ILog _log;

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
            ConfigureMapping<BuyerTransferredToNewLotSaga_RemoveExistingHomeOwner>(x => x.SaleId).ToSaga(x => x.SaleId);
            ConfigureMapping<HomeBuyerDetailsResponse>(x => x.ContactId).ToSaga(x => x.ContactId);
            ConfigureMapping<JobSaleDetailsResponse>(x => x.SaleId).ToSaga(x => x.SaleId);
        }

        public void Handle(BuyerTransferredToNewLot message)
        {
            _log.InfoFormat("Received BuyerTransferredToNewLot for PreviousJob {0} to NewJob {1} for ContactId {2}", message.PreviousJobNumber, message.NewJobNumber, message.ContactId);
            Data.NewJobNumber = message.NewJobNumber;
            Data.PreviousJobNumber = message.PreviousJobNumber;
            Data.ContactId = message.ContactId;
            Data.SaleId = message.SaleId;

            var homeOwner = _homeOwnerService.GetHomeOwnerByJobNumber(message.PreviousJobNumber);
            if (homeOwner == null)
            {
                _log.InfoFormat("HomeOwner was not found in Warranty for job {0}, requesting JobSaleDetails from TIPS since we do not need to remove an existing owner", message.PreviousJobNumber);
                // Get the updated Job details from TIPS
                Bus.Send(new RequestJobSaleDetails { SaleId = Data.SaleId });
                return;
            }

            _log.InfoFormat("Existing HomeOwner found in Warranty for job {0}, proceeding to remove them from the job.", message.PreviousJobNumber);
            Data.HomeOwnerReference = homeOwner.HomeOwnerId;
            Bus.SendLocal(new BuyerTransferredToNewLotSaga_RemoveExistingHomeOwner(Data.SaleId));
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
            Bus.Send(new RequestJobSaleDetails { SaleId = Data.SaleId });
        }

        public void Handle(JobSaleDetailsResponse message)
        {
            _log.InfoFormat("Received JobDetailsResponse from TIPS for job number {0}, will now create job in Warranty.", message.JobNumber);

            var job = _jobService.GetJobByNumber(message.JobNumber);
            if (job == null)
            {
                _log.InfoFormat("Job {0} does not exist, will create a new job with details from TIPS.", message.JobNumber);

                job = Mapper.Map<Job>(message);
                job.CreatedBy = Constants.ENDPOINT_NAME;
                job.UpdatedBy = Constants.ENDPOINT_NAME;
                job.CreatedDate = DateTime.UtcNow;
                job.UpdatedDate = DateTime.UtcNow;
            }
            else
            {
                _log.InfoFormat("Job {0} exists, will update the existing job with details from TIPS.", message.JobNumber);
                job = Mapper.Map(message, job);
                job.UpdatedBy = Constants.ENDPOINT_NAME;
                job.UpdatedDate = DateTime.UtcNow;
            }

            var builder = _employeeService.GetEmployeeByNumber(message.BuilderEmployeeID);
            if (builder != null)
            {
                _log.InfoFormat("Found a builder from the JobDetailsResponse for employeeid {0}, assigning it to the job.", message.BuilderEmployeeID);
                job.BuilderEmployeeId = builder.EmployeeId;
            }

            var community = _communityService.GetCommunityByNumber(message.CommunityNumber);
            if (community != null)
            {
                _log.InfoFormat("Found a community from the JobDetailsResponse for CommunityNumber {0}, assigning it to the job.", message.CommunityNumber);
                job.CommunityId = community.CommunityId;
            }

            if (job.IsNew())
            {
                _log.InfoFormat("Saving new job record in Warranty for job number {0}.", message.JobNumber);
                job = _jobService.CreateJob(job);
            }
            else
            {
                _log.InfoFormat("Updating job {0} in Warranty with new details from TIPS.", message.JobNumber);
                _jobService.Save(job);
            }

            Data.JobIdReference = job.JobId;

            _log.InfoFormat("Requesting HomeBuyerDetails for Contact {0} on JobNumber {1}.", Data.ContactId, job.JobNumber);
            Bus.Send(new RequestHomeBuyerDetails(Data.ContactId));
        }

        public void Handle(HomeBuyerDetailsResponse message)
        {
            _log.InfoFormat("Response for HomeBuyerDetails received from TIPS for ContactId {0} - {1}", message.ContactId, JsonConvert.SerializeObject(message));

            var job = _jobService.GetJobById(Data.JobIdReference);

            var homeOwner = _homeOwnerService.GetByHomeOwnerId(Data.HomeOwnerReference);
            if (homeOwner == null)
            {
                homeOwner = Mapper.Map<HomeOwner>(message);
                homeOwner.HomeOwnerId = Guid.NewGuid();
                homeOwner.HomeOwnerNumber = 1;
                homeOwner.CreatedBy = Constants.ENDPOINT_NAME;
                homeOwner.UpdatedBy = Constants.ENDPOINT_NAME;
                homeOwner.CreatedDate = DateTime.UtcNow;
                homeOwner.UpdatedDate = DateTime.UtcNow;
                homeOwner.JobId = Data.JobIdReference; // required or it will violate a known unique index

                _log.InfoFormat("Creating new HomeOwner in Warranty from TIPS information = {0}", JsonConvert.SerializeObject(homeOwner));
                homeOwner = _homeOwnerService.Create(homeOwner);
            }
            else
            {
                _log.InfoFormat("Updating existing HomeOwner in Warranty from TIPS information = {0}", JsonConvert.SerializeObject(homeOwner));
                homeOwner = Mapper.Map(message, homeOwner); // update with latest data from TIPS
                homeOwner.UpdatedBy = Constants.ENDPOINT_NAME;
                homeOwner.UpdatedDate = DateTime.UtcNow;
            }

            _homeOwnerService.AssignToJob(homeOwner, job);
            _log.InfoFormat("Assigned HomeOwner {0} to JobNumber {1}.", homeOwner.HomeOwnerNumber, job.JobNumber);

            _taskService.CreateTasks(job.JobId);
            _log.InfoFormat("Created tasks for JobNumber {0}.", job.JobNumber);

            MarkAsComplete();
            _log.InfoFormat("BuyerTransferredToNewLogSaga complete for JobNumber {0}.", job.JobNumber);
        }
    }

    public class BuyerTransferredToNewLotSaga_RemoveExistingHomeOwner : IBusCommand
    {
        public long SaleId { get; set; }

        public BuyerTransferredToNewLotSaga_RemoveExistingHomeOwner() { }

        public BuyerTransferredToNewLotSaga_RemoveExistingHomeOwner(long saleId)
        {
            SaleId = saleId;
        }
    }

    public class BuyerTransferredToNewLotSagaData : IContainSagaData
    {
        public virtual Guid Id { get; set; }
        public virtual string Originator { get; set; }
        public virtual string OriginalMessageId { get; set; }

        public virtual string NewJobNumber { get; set; }
        public virtual string PreviousJobNumber { get; set; }

        [Unique]
        public virtual Guid ContactId { get; set; }
        public virtual long SaleId { get; set; }
        public virtual Guid JobIdReference { get; set; }
        public virtual Guid HomeOwnerReference { get; set; }
    }
    
}