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
        IHandleMessages<HomeBuyerDetailsResponse>
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
                _log.InfoFormat("Community was not found for CommunityNumber {0}, requesting it from Accounting", Data.JobSaleDetails.CommunityNumber);
                Bus.SendLocal(new HomeSoldSaga_GetCommunityDetails(Data.SaleId));
                return;
            }

            _log.InfoFormat("Community found for CommunityNumber {0} on Sale {1}, proceeding to handle job details", Data.JobSaleDetails.CommunityNumber, message.SaleId);

            Data.CommunityReferenceId = community.CommunityId;
            Bus.SendLocal(new HomeSoldSaga_CreateOrUpdateJob(Data.SaleId));
        }

        public void Handle(HomeSoldSaga_GetCommunityDetails message)
        {
            var community = _communityService.GetCommunityByNumber(Data.JobSaleDetails.CommunityNumber);
            if (community != null)
            {
                _log.InfoFormat("Community with CommunityNumber {0} appears to have been added already.  Skipping creation of community from response.", Data.JobSaleDetails.CommunityNumber);
                Data.CommunityReferenceId = community.CommunityId;
                Bus.SendLocal(new HomeSoldSaga_CreateOrUpdateJob(Data.SaleId));
                return;
            }

            _log.InfoFormat("Requesting Community data from Accounting for CommunityNumber {0} on Sale {1}", Data.JobSaleDetails.CommunityNumber, message.SaleId);
            using (var client = new HttpClient())
            {
                client.BaseAddress = AccountingBaseAddress;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.dwh.accounting-v1+json"));
                
                var result = client.GetAsync(string.Format("community?communitynumber={0}", Data.JobSaleDetails.CommunityNumber)).Result;
                result.EnsureSuccessStatusCode();

                var details = JsonConvert.DeserializeObject<CommunityDetails>(result.Content.ReadAsStringAsync().Result);
                _log.InfoFormat("Received Community data from Accounting for CommunityNumber {0} on Sale {1}", Data.JobSaleDetails.CommunityNumber, message.SaleId);

                var city = _communityService.GetCity(details.Market.JDEId);
                if (city == null)
                {
                    _log.InfoFormat("Creating missing City record from Community data for CommunityNumber {0} on Sale {1}", Data.JobSaleDetails.CommunityNumber, message.SaleId);
                    city = Mapper.Map<City>(details);
                    city = _communityService.CreateCity(city);
                }

                var division = _communityService.GetDivision(details.Division.JDEId);
                if (division == null)
                {
                    _log.InfoFormat("Creating missing Division record from Community data for CommunityNumber {0} on Sale {1}", Data.JobSaleDetails.CommunityNumber, message.SaleId);
                    division = Mapper.Map<Division>(details);
                    division = _communityService.CreateDivision(division);
                }

                var project = _communityService.GetProject(details.Project.JDEId);
                if (project == null)
                {
                    _log.InfoFormat("Creating missing Project record from Community data for CommunityNumber {0} on Sale {1}", Data.JobSaleDetails.CommunityNumber, message.SaleId);
                    project = Mapper.Map<Project>(details);
                    project = _communityService.CreateProject(project);
                }

                community = Mapper.Map<Community>(details);
                community.CityId = city.CityId;
                community.DivisionId = division.DivisionId;
                community.ProjectId = project.ProjectId;

                _log.InfoFormat("Creating Community from data for CommunityNumber {0} on Sale {1}", Data.JobSaleDetails.CommunityNumber, message.SaleId);
                community = _communityService.Create(community);
                Data.CommunityReferenceId = community.CommunityId;
            }

            _log.InfoFormat("Proceeding to handle job details for CommunityNumber {0} on Sale {1}", Data.JobSaleDetails.CommunityNumber, message.SaleId);
            Bus.SendLocal(new HomeSoldSaga_CreateOrUpdateJob(Data.SaleId));
        }

        public void Handle(HomeSoldSaga_CreateOrUpdateJob message)
        {
            _log.InfoFormat("Handling job details for sale {0}, checking for job with job number {1}", message.SaleId, Data.JobNumber);
            var job = _jobService.GetJobByNumber(Data.JobNumber);
            if (job == null)
            {
                _log.InfoFormat("Job was not found for job number {0}, proceeding to create new job", Data.JobNumber);

                job = Mapper.Map<Job>(Data.JobSaleDetails);
                job.CreatedBy = Constants.ENDPOINT_NAME;
                job.UpdatedBy = Constants.ENDPOINT_NAME;
                job.CreatedDate = DateTime.UtcNow;
                job.UpdatedDate = DateTime.UtcNow;
                job.CommunityId = Data.CommunityReferenceId;

                var builder = _employeeService.GetEmployeeByNumber(Data.JobSaleDetails.BuilderEmployeeID);
                if (builder != null)
                    job.BuilderEmployeeId = builder.EmployeeId;

                job = _jobService.CreateJob(job);
                _log.InfoFormat("New job created with job number {0} for sale {1}", Data.JobNumber, message.SaleId);
            }
            else
            {
                _log.InfoFormat("Job found for job number {0}, proceeding to update the existing job", Data.JobNumber);

                // Update properties for existing job from what we received from TIPS
                job = Mapper.Map(Data.JobSaleDetails, job);
                job.UpdatedBy = Constants.ENDPOINT_NAME;
                job.UpdatedDate = DateTime.UtcNow;

                var salesConsultant = _employeeService.GetEmployeeByNumber(Data.JobSaleDetails.SalesConsultantEmployeeID);
                if (salesConsultant != null)
                    job.SalesConsultantEmployeeId = salesConsultant.EmployeeId;

                if (Data.JobSaleDetails.CloseDate.HasValue)
                {
                    job.CloseDate = Data.JobSaleDetails.CloseDate;
                    job.WarrantyExpirationDate = Data.JobSaleDetails.CloseDate.Value.AddYears(10);
                }

                _jobService.UpdateExistingJob(job);
                _log.InfoFormat("Existing job with job number {0} for sale {1} was updated with new details", Data.JobNumber, message.SaleId);
            }

            Data.JobReferenceId = job.JobId;

            // If there is an existing owner, we need to remove them
            _log.InfoFormat("Removing any existing homeowner from job {0} for sale {1}", Data.JobNumber, message.SaleId);
            _homeOwnerService.RemoveHomeOwner(job);

            // Request the new HomeOwner info from TIPS
            _log.InfoFormat("Requesting new homeowner details for contact {0} on sale {1} from TIPS", Data.ContactId, message.SaleId);
            Bus.Send(new RequestHomeBuyerDetails(Data.ContactId));
        }

        public void Handle(HomeBuyerDetailsResponse message)
        {
            _log.InfoFormat("Received homeowner details for contact {0} on sale {1} from TIPS", Data.ContactId, Data.SaleId);

            var job = _jobService.GetJobById(Data.JobReferenceId);
            var existingHomeOwner = _homeOwnerService.GetHomeOwnerByJobNumber(job.JobNumber);
            HomeOwner homeOwner; 

            if (existingHomeOwner == null)
            {
                homeOwner = Mapper.Map<HomeOwner>(message);
                homeOwner.HomeOwnerId = Guid.NewGuid();
                homeOwner.HomeOwnerNumber = 1;
                homeOwner.CreatedBy = Constants.ENDPOINT_NAME;
                homeOwner.UpdatedBy = Constants.ENDPOINT_NAME;
                homeOwner.CreatedDate = DateTime.UtcNow;
                homeOwner.UpdatedDate = DateTime.UtcNow;
                homeOwner.JobId = Data.JobReferenceId;

                homeOwner = _homeOwnerService.Create(homeOwner);
                _log.InfoFormat("Created homeowner record for contact {0} on sale {1}", Data.ContactId, Data.SaleId);
            }
            else
            {
                homeOwner = Mapper.Map<HomeOwner>(existingHomeOwner);
                homeOwner.UpdatedBy = Constants.ENDPOINT_NAME;
                homeOwner.UpdatedDate = DateTime.UtcNow;
                homeOwner.JobId = Data.JobReferenceId;

                _homeOwnerService.UpdateHomeOwner(homeOwner);
                _log.InfoFormat("Homeowner already exists, updating homeowner record for contact {0} on sale {1} for job {2}", Data.ContactId, 
                    Data.SaleId, 
                    Data.JobReferenceId);
            }

            _log.InfoFormat("Assigning new homeowner to sale {0} with job number {1}", Data.SaleId, Data.JobNumber);
            _homeOwnerService.AssignToJob(homeOwner, job);

            _log.InfoFormat("Creating tasks for sale {0} with job number {1}", Data.SaleId, Data.JobNumber);
            _taskService.CreateTasks(Data.JobReferenceId);

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
        public virtual long SaleId { get; set; }
        public virtual JobSaleDetailsResponse JobSaleDetails { get; set; }
        public virtual Guid CommunityReferenceId { get; set; }
        public virtual Guid JobReferenceId { get; set; }
    }
}