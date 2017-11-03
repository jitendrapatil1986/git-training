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
using Warranty.Core;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;
using Warranty.Core.Features.Homeowner;
using Warranty.Core.Services;
using Warranty.Server.Handlers.Jobs;

namespace Warranty.Server.Sagas
{
    public class BuyerTransferredToNewLotSaga : Saga<BuyerTransferredToNewLotSagaData>,
        IAmStartedByMessages<BuyerTransferredToNewLot>,
        IHandleMessages<HomeBuyerDetailsResponse>,
        IHandleMessages<JobSaleDetailsResponse>
    {
        private readonly IJobService _jobService;
        private readonly IHomeOwnerService _homeOwnerService;
        private readonly ITaskService _taskService;
        private readonly IEmployeeService _employeeService;
        private readonly ICommunityService _communityService;
        private readonly ILog _log;
        private readonly IMediator _mediator;

        public BuyerTransferredToNewLotSaga(IJobService jobService, IHomeOwnerService homeOwnerService, ITaskService taskService, IEmployeeService employeeService, ICommunityService communityService, ILog log, IMediator mediator)
        {
            _jobService = jobService;
            _homeOwnerService = homeOwnerService;
            _taskService = taskService;
            _employeeService = employeeService;
            _communityService = communityService;
            _log = log;
            _mediator = mediator;
        }

        public BuyerTransferredToNewLotSaga() { }

        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping<HomeBuyerDetailsResponse>(x => x.ContactId).ToSaga(x => x.ContactId);
            ConfigureMapping<JobSaleDetailsResponse>(x => x.SaleId).ToSaga(x => x.SaleId);
        }

        public void Handle(BuyerTransferredToNewLot message)
        {
            _log.InfoFormat("Received BuyerTransferredToNewLot for PreviousJob {0} to NewJob {1} for ContactId {2} on SaleId {3}", message.PreviousJobNumber, message.NewJobNumber, message.ContactId, message.SaleId);
            Data.NewJobNumber = message.NewJobNumber;
            Data.PreviousJobNumber = message.PreviousJobNumber;
            Data.ContactId = message.ContactId;
            Data.SaleId = message.SaleId;

            _log.InfoFormat("Requesting current job information from TIPS for SaleId {0}", message.SaleId);
            Bus.Send(new RequestJobSaleDetails { SaleId = Data.SaleId });
        }

        public void Handle(JobSaleDetailsResponse message)
        {
            _log.InfoFormat("Received JobDetailsResponse from TIPS for SaleId {0}", message.SaleId);

            if (string.IsNullOrWhiteSpace(message.JobNumber) || message.JobNumber != Data.NewJobNumber)
            {
                _log.ErrorFormat("The Job information received from TIPS has a job number {0} that doesn't match the orignal job number from the event {1}.  This suggests something happened to the job since the event was fired.", message.JobNumber, Data.NewJobNumber);
                _log.ErrorFormat("Ending BuyerTransfer saga for Sale {0} due to invalid data condition.", Data.SaleId);
                MarkAsComplete();
                return;
            }

            var job = _jobService.GetJobByNumber(message.JobNumber);
            if (job == null)
            {
                _log.InfoFormat("Job {0} does not exist locally, will create a new job with details from TIPS.", message.JobNumber);
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

            _log.InfoFormat("Requesting updated HomeBuyer details for Contact {0} on SaleId {1}.", Data.ContactId, Data.SaleId);
            Bus.Send(new RequestHomeBuyerDetails(Data.ContactId));
        }

        public void Handle(HomeBuyerDetailsResponse message)
        {
            _log.InfoFormat("Response for HomeBuyerDetails received from TIPS for ContactId {0} - {1}", message.ContactId, JsonConvert.SerializeObject(message));

            var newJob = _jobService.GetJobById(Data.JobIdReference);
            var previousJob = _jobService.GetJobByNumber(Data.PreviousJobNumber);
            var previousHomeOwner = _homeOwnerService.GetHomeOwnerByJobNumber(Data.PreviousJobNumber);

            if (newJob.CurrentHomeOwnerId.HasValue)
            {
                _log.ErrorFormat("The new job {0} on SaleId {1} currently has a home owner assigned in Warranty.  You cannot transfer to a job that is currently assigned a home owner.  This suggests something changed or failed to update prior to this message being handled.", Data.NewJobNumber, Data.SaleId);
                MarkAsComplete();
                return;
            }

            // If we have a homeowner locally, we just want to update them and point them to the new job
            if (previousHomeOwner != null)
            {
                _log.ErrorFormat("Homeowner {3} was found on previous job {0} for SaleId {1}, updating their details and removing them from the previous job.", Data.PreviousJobNumber, Data.SaleId, previousHomeOwner.HomeOwnerName);

                previousHomeOwner = Mapper.Map(message, previousHomeOwner);  // Update the contact info from TIPS that we just received
                _homeOwnerService.RemoveFromJob(previousHomeOwner, previousJob);

                _log.InfoFormat("Removing tasks from previous job {0} on SaleId {1}", Data.PreviousJobNumber, Data.SaleId);
                _taskService.DeleteTask(previousJob.JobId, TaskType.QualityIntroductionofWSR);
                _taskService.DeleteTask(previousJob.JobId, TaskType.JobStage10JobClosed);

                _log.InfoFormat("Creating tasks for previous job {0} on SaleId {1}", Data.PreviousJobNumber, Data.SaleId);
                _taskService.CreateTasks(previousJob.JobId);
            }
            // Since we don't have a homeowner locally, we need to create a new record and assign them to the new job
            else
            {
                _log.ErrorFormat("Homeowner was not found on previous job {0} for SaleId {1}, creating a new HomeOwner record for them.", Data.PreviousJobNumber, Data.SaleId);

                var createCommand = Mapper.Map<CreateNewHomeOwnerCommand>(message);
                createCommand.JobId = Data.JobIdReference; // required or it will violate a known unique index

                previousHomeOwner = _mediator.Send(createCommand);
            }

            _log.InfoFormat("Assigning HomeOwner {0} to JobNumber {1} on SaleId {2}.", previousHomeOwner.HomeOwnerName, newJob.JobNumber, Data.SaleId);
            _homeOwnerService.AssignToJob(previousHomeOwner, newJob);

            _log.InfoFormat("Creating tasks for JobNumber {0} on SaleId {1}.", newJob.JobNumber, Data.SaleId);
            _taskService.CreateTasks(newJob.JobId);
            
            MarkAsComplete();
            _log.InfoFormat("BuyerTransferredToNewLogSaga complete for JobNumber {0}.", newJob.JobNumber);
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
    }
    
}