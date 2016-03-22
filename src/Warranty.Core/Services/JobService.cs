using System;
using NPoco;
using TIPS.Events.Models;
using Job = Warranty.Core.Entities.Job;
using TipsJob = TIPS.Events.Models.Job;

namespace Warranty.Core.Services
{
    public class JobService : IJobService
    {
        private readonly IDatabase _database;
        private readonly IEmployeeService _employeeService;
        private readonly ICommunityService _communityService;

        public JobService(IDatabase database, IEmployeeService employeeService, ICommunityService communityService)
        {
            if(database == null)
                throw new ArgumentNullException("database");
            if (employeeService == null)
                throw new ArgumentNullException("employeeService");
            if (communityService == null)
                throw new ArgumentNullException("communityService");

            _database = database;
            _employeeService = employeeService;
            _communityService = communityService;
        }

        public Job GetJobById(Guid jobId)
        {
            return _database.SingleById<Job>(jobId);
        }

        public bool IsModelOrShowcase(Job job)
        {
            return job.CurrentHomeOwnerId == null;
        }

        public Job GetJobByNumber(string jobNumber)
        {
            if (jobNumber == null)
                throw new ArgumentNullException("jobNumber");

            return _database.SingleOrDefault<Job>("WHERE JobNumber = @0", jobNumber);
        }

        public void Save(Job job)
        {
            using (_database)
            {
                _database.Update(job);
            }
        }

        public Job CreateJob(Job job)
        {
            if (job == null)
                throw new ArgumentNullException("job");

            using (_database)
            {
                _database.Insert(job);
            }

            return job;
        }

        

        public Job CreateJob(TipsJob tipsJob)
        {
            if(tipsJob == null)
                throw new ArgumentNullException("tipsJob");

            var job = UpdateJobFromTipsJob(new Job(), tipsJob);
            return CreateJob(job);
        }

        public Job CreateJob(Sale sale)
        {
            if (sale == null)
                throw new ArgumentNullException("sale");

            var job = UpdateJobFromSale(new Job(), sale);
            return CreateJob(job);
        }
        public void UpdateExistingJob(Job job)
        {
            using (_database)
            {
                _database.Update(job);
            }
        }

        public void UpdateExistingJob(Job job, Sale sale)
        {
            if(job == null)
                throw new ArgumentNullException("job");
            if(sale == null)
                throw new ArgumentNullException("sale");

            var updatedJob = UpdateJobFromSale(job, sale);
            UpdateExistingJob(updatedJob);
        }

        public void UpdateExistingJob(Job job, TipsJob tipsJob)
        {
            if (job == null)
                throw new ArgumentNullException("job");
            if (tipsJob == null)
                throw new ArgumentNullException("tipsJob");

            var updatedJob = UpdateJobFromTipsJob(job, tipsJob);
            UpdateExistingJob(updatedJob);
        }

        private Job UpdateJobFromTipsJob(Job job, TipsJob tipsJob)
        {
            if (job == null)
                throw new ArgumentNullException("job");
            if (tipsJob == null)
                throw new ArgumentNullException("tipsJob");

            var builder = _employeeService.GetEmployeeByNumber(tipsJob.BuilderEmployeeID);
            var community = _communityService.GetCommunityByNumber(tipsJob.CommunityNumber);

            job.JobNumber = tipsJob.JobNumber;
            job.PlanNumber = tipsJob.PlanNumber;
            job.Elevation = tipsJob.Elevation;
            job.AddressLine = tipsJob.AddressLine1;
            job.City = tipsJob.AddressCity;
            job.StateCode = tipsJob.AddressStateAbbreviation;
            job.PostalCode = tipsJob.AddressZipCode;
            job.PlanType = tipsJob.JobType;
            job.CreatedBy = "Warranty.Server";
            job.CreatedDate = DateTime.Now;
            job.JdeIdentifier = tipsJob.JobNumber;
            job.PlanName = tipsJob.PlanName;
            job.PlanTypeDescription = null;
            job.Swing = tipsJob.Swing;

            if (tipsJob.LegalDescription != null)
            {
                job.LegalDescription = tipsJob.LegalDescription.ToString();
            }
            else
            {
                job.LegalDescription = null;
            }
            if (community != null)
            {
                job.CommunityId = community.CommunityId;
            }
            if (builder != null)
            {
                job.BuilderEmployeeId = builder.EmployeeId;
            }
            
            if (tipsJob.Stage.HasValue)
            {
                job.Stage = tipsJob.Stage.Value;
            }
            
            return job;
        }

        private Job UpdateJobFromSale(Job job, Sale sale)
        {
            if (job == null)
                throw new ArgumentNullException("job");
            if (sale == null)
                throw new ArgumentNullException("sale");

            UpdateJobFromTipsJob(job, sale);

            var salesConsultant = _employeeService.GetEmployeeByNumber(sale.SalesConsultantEmployeeID);
            if (salesConsultant != null)
            {
                job.SalesConsultantEmployeeId = salesConsultant.EmployeeId;
            }
            if (sale.CloseDate.HasValue)
            {
                job.CloseDate = sale.CloseDate;
                job.WarrantyExpirationDate = sale.CloseDate.Value.AddYears(10);
            }
            return job;
        }
    }
}