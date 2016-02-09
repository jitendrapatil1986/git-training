using System;
using NPoco;
using TIPS.Events.Models;
using Job = Warranty.Core.Entities.Job;

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

        private Job UpdateJobFromSale(Job job, Sale sale)
        {
            if(job == null)
                throw new ArgumentNullException("job");
            if (sale == null)
                throw new ArgumentNullException("sale");

            var builder = _employeeService.GetEmployeeByNumber(sale.BuilderEmployeeID);
            var salesConsultant = _employeeService.GetEmployeeByNumber(sale.SalesConsultantEmployeeID);
            var community = _communityService.GetCommunityByNumber(sale.CommunityNumber);

            job.JobNumber = sale.JobNumber;
            job.PlanNumber = sale.PlanNumber;
            job.Elevation = sale.Elevation;
            job.AddressLine = sale.AddressLine1;
            job.City = sale.AddressCity;
            job.StateCode = sale.AddressStateAbbreviation;
            job.PostalCode = sale.AddressZipCode;
            job.PlanType = sale.JobType;
            job.CloseDate = sale.CloseDate;
            job.CreatedBy = "Warranty.Server";
            job.CreatedDate = DateTime.Now;
            job.JdeIdentifier = sale.JobNumber;
            job.PlanName = sale.PlanName;
            job.PlanTypeDescription = null;
            job.Swing = sale.Swing;

            if (sale.LegalDescription != null)
            {
                job.LegalDescription = sale.LegalDescription.ToString();
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
            if (salesConsultant != null)
            {
                job.SalesConsultantEmployeeId = salesConsultant.EmployeeId;
            }
            if (sale.Stage.HasValue)
            {
                job.Stage = sale.Stage.Value;
            }
            if (sale.CloseDate.HasValue)
            {
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
    }
}