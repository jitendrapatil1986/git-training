using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
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
using Warranty.Core.Services;
using Warranty.Core.Services.Models;
using Warranty.Server.Handlers.Jobs;

namespace Warranty.Server.Sagas
{
    public class HomeSoldSaga : Saga<HomeSoldSagaData>,
        IAmStartedByMessages<HomeSold>,
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
        private readonly ILog _log;

        public Uri AccountingBaseAddress
        {
            get
            {
                return new Uri(ConfigurationManager.AppSettings["Accounting.API.BaseUri"]);
            }
        }

        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping<HomeSoldSaga_GetCommunityDetails>(m => m.SaleId).ToSaga(s => s.SaleId);
            ConfigureMapping<JobSaleDetailsResponse>(m => m.SaleId).ToSaga(s => s.SaleId);
            ConfigureMapping<HomeSoldSaga_CreateOrUpdateJob>(m => m.SaleId).ToSaga(s => s.SaleId);
            ConfigureMapping<HomeBuyerDetailsResponse>(m => m.ContactId).ToSaga(s => s.ContactId);
            ConfigureMapping<HomeSoldSaga_AssignHomeOwnerToJob>(m => m.SaleId).ToSaga(s => s.SaleId);
        }

        public HomeSoldSaga() { }

        public HomeSoldSaga(ICommunityService communityService, IJobService jobService, IEmployeeService employeeService, IHomeOwnerService homeOwnerService, ITaskService taskService, ILog log)
        {
            _communityService = communityService;
            _jobService = jobService;
            _employeeService = employeeService;
            _homeOwnerService = homeOwnerService;
            _taskService = taskService;
            _log = log;
        }

        public void Handle(HomeSold message)
        {
            _log.InfoFormat("Received HomeSold for job {0} and saleId {1}", message.JobNumber, message.SaleId);
            Data.ContactId = message.ContactId;
            Data.JobNumber = message.JobNumber;
            Data.SaleId = message.SaleId;

            _log.InfoFormat("Requesting JobSaleDetails from TIPS for saleId {0}", message.SaleId);
            Bus.Send(new RequestJobSaleDetails {SaleId = Data.SaleId});
        }

        public void Handle(JobSaleDetailsResponse message)
        {
            _log.InfoFormat("Received response for JobSaleDetails from TIPS for saleId {0} with job number {1}", message.SaleId, message.JobNumber);
            Data.JobSaleDetails = message; // Will need info from this later

            var community = _communityService.GetCommunityByNumber(Data.JobSaleDetails.CommunityNumber);
            if (community == null)
            {
                _log.ErrorFormat("Community was not found for CommunityNumber {0} on Sale {1}; currently the Community API is not implemented so this request will be terminated since we cannot proceed.", Data.JobSaleDetails.CommunityNumber, message.SaleId);
                MarkAsComplete();

                //_log.InfoFormat("Community was not found for CommunityNumber {0}, requesting it from Accounting", Data.JobSaleDetails.CommunityNumber);
                //Bus.SendLocal(new HomeSoldSaga_GetCommunityDetails(Data.SaleId));
                // This is what we'll do later
                return;
            }

            _log.InfoFormat("Community found for CommunityNumber {0} on Sale {1}, proceeding to handle job details", Data.JobSaleDetails.CommunityNumber, message.SaleId);
            Data.Community = community;
            Bus.SendLocal(new HomeSoldSaga_CreateOrUpdateJob(Data.SaleId));
        }

        public void Handle(HomeSoldSaga_GetCommunityDetails message)
        {
            _log.InfoFormat("Requesting Community data from Accounting for CommunityNumber {0} on Sale {1}", Data.JobSaleDetails.CommunityNumber, message.SaleId);
            using (var client = new HttpClient())
            {
                client.BaseAddress = AccountingBaseAddress;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.dwh.accounting-v1+json"));
                
                var result = client.GetAsync(string.Format("api/community?communitynumber={0}", Data.JobSaleDetails.CommunityNumber)).Result;
                result.EnsureSuccessStatusCode();

                var details = JsonConvert.DeserializeObject<CommunityDetails>(result.Content.ReadAsStringAsync().Result);
                var newCommunity = Mapper.Map<Community>(details);

                Data.Community = _communityService.Create(newCommunity);
            }

            _log.InfoFormat("Received Community data from Accounting for CommunityNumber {0} on Sale {1}, proceeding to handle job details", Data.JobSaleDetails.CommunityNumber, message.SaleId);
            Bus.SendLocal(new HomeSoldSaga_CreateOrUpdateJob(Data.SaleId));
        }

        

        public void Handle(HomeSoldSaga_CreateOrUpdateJob message)
        {
            _log.InfoFormat("Handling job details for sale {0}, checking for job with job number {1}", message.SaleId, Data.JobNumber);
            var existingJob = _jobService.GetJobByNumber(Data.JobNumber);
            if (existingJob == null)
            {
                _log.InfoFormat("Job was not found for job number {0}, proceeding to create new job", Data.JobNumber);

                Data.NewJob = Mapper.Map<Job>(Data.JobSaleDetails);
                Data.NewJob.CreatedBy = Constants.ENDPOINT_NAME;
                Data.NewJob.UpdatedBy = Constants.ENDPOINT_NAME;
                Data.NewJob.CreatedDate = DateTime.UtcNow;
                Data.NewJob.UpdatedDate = DateTime.UtcNow;
                Data.NewJob.CommunityId = Data.Community.CommunityId;

                var builder = _employeeService.GetEmployeeByNumber(Data.JobSaleDetails.BuilderEmployeeID);
                if (builder != null)
                    Data.NewJob.BuilderEmployeeId = builder.EmployeeId;

                Data.NewJob = _jobService.CreateJob(Data.NewJob);
                _log.InfoFormat("New job created with job number {0} for sale {1}", Data.JobNumber, message.SaleId);
            }
            else
            {
                _log.InfoFormat("Job found for job number {0}, proceeding to update the existing job", Data.JobNumber);

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
                _log.InfoFormat("Existing job with job number {0} for sale {1} was updated with new details", Data.JobNumber, message.SaleId);
            }

            // If there is an existing owner, we need to remove them
            _log.InfoFormat("Removing any existing homeowner from job {0} for sale {1}", Data.JobNumber, message.SaleId);
            _homeOwnerService.RemoveHomeOwner(Data.NewJob);


            // Request the new HomeOwner info from TIPS
            _log.InfoFormat("Requesting new homeowner details for contact {0} on sale {1} from TIPS", Data.ContactId, message.SaleId);
            Bus.Send(new RequestHomeBuyerDetails(Data.ContactId));
        }

        public void Handle(HomeBuyerDetailsResponse message)
        {
            _log.InfoFormat("Received homeowner details for contact {0} on sale {1} from TIPS", Data.ContactId, Data.SaleId);

            var homeOwner = Mapper.Map<HomeOwner>(message);
            homeOwner.HomeOwnerId = Guid.NewGuid();
            homeOwner.HomeOwnerNumber = 1;
            homeOwner.CreatedBy = Constants.ENDPOINT_NAME;
            homeOwner.UpdatedBy = Constants.ENDPOINT_NAME;
            homeOwner.CreatedDate = DateTime.UtcNow;
            homeOwner.UpdatedDate = DateTime.UtcNow;

            Data.HomeOwner = _homeOwnerService.Create(homeOwner);
            _log.InfoFormat("Created homeowner record for contact {0} on sale {1}", Data.ContactId, Data.SaleId);

            _log.InfoFormat("Proceeding to assign new homeowner to sale {0}", Data.SaleId);
            Bus.SendLocal(new HomeSoldSaga_AssignHomeOwnerToJob(Data.SaleId));
        }

        public void Handle(HomeSoldSaga_AssignHomeOwnerToJob message)
        {
            _log.InfoFormat("Assigning new homeowner to sale {0} with job number {1}", Data.SaleId, Data.JobNumber);
            _homeOwnerService.AssignToJob(Data.HomeOwner, Data.NewJob);

            _log.InfoFormat("Creating tasks for sale {0} with job number {1}", Data.SaleId, Data.JobNumber);
            _taskService.CreateTasks(Data.NewJob.JobId);

            MarkAsComplete();
            _log.InfoFormat("Completed handling of HomeSold for sale {0} with job number {1}", Data.SaleId, Data.JobNumber);
        }
    }

    public class HomeSoldSaga_AssignHomeOwnerToJob : IBusCommand
    {
        public long SaleId { get; set; }

        public HomeSoldSaga_AssignHomeOwnerToJob() { }
        public HomeSoldSaga_AssignHomeOwnerToJob(long saleId)
        {
            SaleId = saleId;
        }
    }

    public class HomeSoldSaga_CreateOrUpdateJob : IBusCommand
    {
        public long SaleId { get; set; }

        public HomeSoldSaga_CreateOrUpdateJob() { }
        public HomeSoldSaga_CreateOrUpdateJob(long saleId)
        {
            SaleId = saleId;
        }
    }

    public class HomeSoldSaga_GetCommunityDetails : IBusCommand
    {
        public long SaleId { get; set; }

        public HomeSoldSaga_GetCommunityDetails() { }
        public HomeSoldSaga_GetCommunityDetails(long saleId)
        {
            SaleId = saleId;
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
        public virtual Job NewJob { get; set; }
        public virtual Community Community { get; set; }
        public virtual JobSaleDetailsResponse JobSaleDetails { get; set; }
        public virtual HomeOwner HomeOwner { get; set; }
        public virtual long SaleId { get; set; }
    }
}