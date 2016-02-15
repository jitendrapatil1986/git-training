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
            _database = database;
            _employeeService = employeeService;
            _communityService = communityService;
        }

        public Job GetJobByNumber(string jobNumber)
        {
            if (jobNumber == null)
                throw new ArgumentNullException("jobNumber");

            return _database.SingleOrDefault<Job>("WHERE JobNumber = @0", jobNumber);
        }

        public Job CreateJobAndInsert(TipsJob tipsJob)
        {
            var job = CreateJobFromTipsJob(tipsJob);
            using (_database)
            {
                _database.Insert(job);
            }
            return job;
        }

        public Job  CreateJobAndInsert(Sale sale)
        {
            var job = CreateJobFromSale(sale);
            using (_database)
            {
                _database.Insert(job);
            }
            return job;
        }

        public void UpdateExistingJob(Job job, Sale sale)
        {
            using (_database)
            {
                var updatedJob = UpdateJobFromSale(job, sale);
                _database.Update(updatedJob);
            }
        }

        public void UpdateExistingJob(Job job, TipsJob tipsJob)
        {
            using (_database)
            {
                var updatedJob = UpdateJobFromTipsJob(job, tipsJob);
                _database.Update(updatedJob);
            }
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
            if(job == null)
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

        public Job CreateJobFromSale(Sale sale)
        {
            if (sale == null)
                throw new ArgumentNullException("sale");

            var job = new Job();
            return UpdateJobFromSale(job, sale);
        }

        public Job CreateJobFromTipsJob(TipsJob tipsJob)
        {
            if (tipsJob == null)
                throw new ArgumentException("job");

            var job = new Job();
            return UpdateJobFromTipsJob(job, tipsJob);
        }
    }
}