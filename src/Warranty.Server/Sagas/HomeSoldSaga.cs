using System;
using AutoMapper;
using Common.Messages;
using NServiceBus;
using NServiceBus.Saga;
using TIPS.Commands.Requests;
using TIPS.Commands.Responses;
using TIPS.Events.JobEvents;
using Warranty.Core.Entities;
using Warranty.Core.Services;
using Warranty.Server.Handlers.Jobs;

namespace Warranty.Server.Sagas
{
    public class HomeSoldSaga : Saga<HomeSoldSagaData>,
        IHandleMessages<HomeSold>,
        IHandleMessages<JobSaleDetailsResponse>,
        IHandleMessages<HomeSoldSaga_GetCommunityDetails>,
        IHandleMessages<HomeSoldSaga_CreateOrUpdateJob>,
        IHandleMessages<HomeBuyerDetailsResponse>,
        IHandleMessages<HomeSoldSaga_AssignHomeOwnerToJob>
    {
        private readonly ICommunityService _communityService;
        private readonly IJobService _jobService;
        private readonly IEmployeeService _employeeService;
        private readonly IHomeOwnerService _homeOwnerService;
        private readonly ITaskService _taskService;

        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping<HomeSoldSaga_GetCommunityDetails>(m => m.JobNumber).ToSaga(s => s.JobNumber);
            ConfigureMapping<JobSaleDetailsResponse>(m => m.JobNumber).ToSaga(s => s.JobNumber);
            ConfigureMapping<HomeSoldSaga_CreateOrUpdateJob>(m => m.JobNumber).ToSaga(s => s.JobNumber);
            ConfigureMapping<HomeBuyerDetailsResponse>(m => m.ContactId).ToSaga(s => s.ContactId);
            ConfigureMapping<HomeSoldSaga_AssignHomeOwnerToJob>(m => m.JobNumber).ToSaga(s => s.JobNumber);
        }

        public HomeSoldSaga(ICommunityService communityService, IJobService jobService, IEmployeeService employeeService, IHomeOwnerService homeOwnerService, ITaskService taskService)
        {
            _communityService = communityService;
            _jobService = jobService;
            _employeeService = employeeService;
            _homeOwnerService = homeOwnerService;
            _taskService = taskService;
        }

        public void Handle(HomeSold message)
        {
            Data.ContactId = message.ContactId;
            Data.JobNumber = message.JobNumber;

            Bus.Send(new RequestJobSaleDetails {JobNumber = Data.JobNumber});
        }

        public void Handle(JobSaleDetailsResponse message)
        {
            Data.CommunityNumber = message.CommunityNumber;
            Data.BuilderEmployeeID = message.BuilderEmployeeID;
            Data.JobSaleDetails = message;

            var community = _communityService.GetCommunityByNumber(Data.CommunityNumber);
            if (community == null)
            {
                Bus.SendLocal(new HomeSoldSaga_GetCommunityDetails(Data.JobNumber));
                return;
            }

            Data.Community = community;
            Bus.SendLocal(new HomeSoldSaga_CreateOrUpdateJob(Data.JobNumber));
        }

        public void Handle(HomeSoldSaga_GetCommunityDetails message)
        {
            // 1. Make Call to Accounting API to get community
            // 2. Create the community in Warranty
            // 3. Set the community on SagaData
            
            Bus.SendLocal(new HomeSoldSaga_CreateOrUpdateJob(Data.JobNumber));
        }

        public void Handle(HomeSoldSaga_CreateOrUpdateJob message)
        {
            var existingJob = _jobService.GetJobByNumber(Data.JobNumber);
            if (existingJob == null)
            {
                Data.NewJob = Mapper.Map<Job>(Data.JobSaleDetails);
                Data.NewJob.CreatedBy = Constants.ENDPOINT_NAME;
                Data.NewJob.UpdatedBy = Constants.ENDPOINT_NAME;
                Data.NewJob.CreatedDate = DateTime.UtcNow;
                Data.NewJob.UpdatedDate = DateTime.UtcNow;
                Data.NewJob.CommunityId = Data.Community.CommunityId;

                var builder = _employeeService.GetEmployeeByNumber(Data.BuilderEmployeeID);
                if (builder != null)
                    Data.NewJob.BuilderEmployeeId = builder.EmployeeId;

                Data.NewJob = _jobService.CreateJob(Data.NewJob);
            }
            else
            {
                // Update properties for existing job from what we received from TIPS
                Data.NewJob = Mapper.Map(Data.JobSaleDetails, existingJob);
                Data.NewJob.UpdatedBy = Constants.ENDPOINT_NAME;
                Data.NewJob.UpdatedDate = DateTime.UtcNow;

                var salesConsultant = _employeeService.GetEmployeeByNumber(Data.JobSaleDetails.SalesConsultantEmployeeID);
                if (salesConsultant != null)
                    Data.NewJob.SalesConsultantEmployeeId = salesConsultant.EmployeeId;

                if (Data.JobSaleDetails.CloseDate.HasValue)
                {
                    Data.NewJob.CloseDate = Data.JobSaleDetails.CloseDate;
                    Data.NewJob.WarrantyExpirationDate = Data.JobSaleDetails.CloseDate.Value.AddYears(10);
                }

                _jobService.UpdateExistingJob(Data.NewJob);
            }

            // If there is an existing owner, we need to remove them
            _homeOwnerService.RemoveHomeOwner(Data.NewJob);

            // Request the new HomeOwner info from TIPS
            Bus.Send(new RequestHomeBuyerDetails(Data.ContactId));
        }

        public void Handle(HomeBuyerDetailsResponse message)
        {
            var homeOwner = Mapper.Map<HomeOwner>(message);
            homeOwner.HomeOwnerId = Guid.NewGuid();
            homeOwner.HomeOwnerNumber = 1;
            homeOwner.CreatedBy = Constants.ENDPOINT_NAME;
            homeOwner.UpdatedBy = Constants.ENDPOINT_NAME;
            homeOwner.CreatedDate = DateTime.UtcNow;
            homeOwner.UpdatedDate = DateTime.UtcNow;

            Data.HomeOwner = _homeOwnerService.Create(homeOwner);
            Bus.SendLocal(new HomeSoldSaga_AssignHomeOwnerToJob(Data.JobNumber));
        }

        public void Handle(HomeSoldSaga_AssignHomeOwnerToJob message)
        {
            _homeOwnerService.AssignToJob(Data.HomeOwner, Data.NewJob);
            _taskService.CreateTasks(Data.NewJob.JobId);
            MarkAsComplete();
        }
    }

    public class HomeSoldSaga_AssignHomeOwnerToJob : IBusCommand
    {
        public string JobNumber { get; set; }

        public HomeSoldSaga_AssignHomeOwnerToJob() { }
        public HomeSoldSaga_AssignHomeOwnerToJob(string jobNumber)
        {
            JobNumber = jobNumber;
        }
    }

    public class HomeSoldSaga_CreateOrUpdateJob : IBusCommand
    {
        public string JobNumber { get; set; }

        public HomeSoldSaga_CreateOrUpdateJob() { }
        public HomeSoldSaga_CreateOrUpdateJob(string jobNumber)
        {
            JobNumber = jobNumber;
        }
    }

    public class HomeSoldSaga_GetCommunityDetails : IBusCommand
    {
        public string JobNumber { get; set; }

        public HomeSoldSaga_GetCommunityDetails() { }
        public HomeSoldSaga_GetCommunityDetails(string jobNumber)
        {
            JobNumber = jobNumber;
        }
    }

    public class HomeSoldSagaData : IContainSagaData
    {
        public virtual Guid Id { get; set; }
        public virtual string Originator { get; set; }
        public virtual string OriginalMessageId { get; set; }
        public virtual Guid ContactId { get; set; }
        [Unique]
        public virtual string JobNumber { get; set; }
        public virtual string CommunityNumber { get; set; }
        public virtual int? BuilderEmployeeID { get; set; }
        public virtual Job NewJob { get; set; }
        public virtual Community Community { get; set; }
        public virtual JobSaleDetailsResponse JobSaleDetails { get; set; }
        public virtual HomeOwner HomeOwner { get; set; }
    }
}