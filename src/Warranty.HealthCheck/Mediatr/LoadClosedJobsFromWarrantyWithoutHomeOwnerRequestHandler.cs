using System;
using MediatR;
using Warranty.HealthCheck.Data;
using Warranty.HealthCheck.Models;

namespace Warranty.HealthCheck.Mediatr
{
    public class LoadClosedJobsFromWarrantyWithoutHomeOwnerRequest : IRequest
    {
        public DateTime MaxCloseDate { get; set; }

        public LoadClosedJobsFromWarrantyWithoutHomeOwnerRequest()
        {
        }

        public LoadClosedJobsFromWarrantyWithoutHomeOwnerRequest(DateTime maxCloseDate)
        {
            MaxCloseDate = maxCloseDate;
        }
    }

    public class LoadClosedJobsFromWarrantyWithoutHomeOwnerRequestHandler : RequestHandler<LoadClosedJobsFromWarrantyWithoutHomeOwnerRequest>
    {
        private readonly IWarrantyDatabase _warrantyDatabase;
        private readonly IHealthCheckDatabase _healthCheckDatabase;

        public LoadClosedJobsFromWarrantyWithoutHomeOwnerRequestHandler(IWarrantyDatabase warrantyDatabase, IHealthCheckDatabase healthCheckDatabase)
        {
            _warrantyDatabase = warrantyDatabase;
            _healthCheckDatabase = healthCheckDatabase;
        }

        protected override void HandleCore(LoadClosedJobsFromWarrantyWithoutHomeOwnerRequest message)
        {
            var closedJobsWithoutHomeOwnerInWarranty = _warrantyDatabase.Fetch<HEALTH_ClosedJobs>(@"SELECT 
		                                                                                            J.JobNumber
                                                                                                    ,@1 AS System
	                                                                                            FROM Warranty.dbo.Jobs J
	                                                                                            LEFT JOIN Warranty.dbo.Homeowners H
		                                                                                            ON H.JobId = J.JobId
	                                                                                            WHERE J.CurrentHomeOwnerId IS NULL
		                                                                                            AND J.CloseDate IS NOT NULL
                                                                                                    AND J.CloseDate > @0
		                                                                                            AND H.HomeOwnerId IS NULL;", message.MaxCloseDate, Systems.Warranty);

            _healthCheckDatabase.InsertBulk(closedJobsWithoutHomeOwnerInWarranty);
        }
    }
}