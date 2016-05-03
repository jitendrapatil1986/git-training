using System;
using MediatR;
using Warranty.HealthCheck.Data;
using Warranty.HealthCheck.Models;

namespace Warranty.HealthCheck.Mediatr
{
    public class LoadSoldJobsFromTipsRequestHandler : RequestHandler<LoadSoldJobsFromTipsRequest>
    {
        private readonly ITipsDatabase _tipsDatabase;
        private readonly IHealthCheckDatabase _healthCheckDatabase;

        public LoadSoldJobsFromTipsRequestHandler(ITipsDatabase tipsDatabase, IHealthCheckDatabase healthCheckDatabase)
        {
            _tipsDatabase = tipsDatabase;
            _healthCheckDatabase = healthCheckDatabase;
        }

        protected override void HandleCore(LoadSoldJobsFromTipsRequest message)
        {
            var tipsJobs = _tipsDatabase.Fetch<HEALTH_SoldJob>(@"SELECT aj.JobNumber, @1 as System
                                                    FROM		SALE_ApprovalJob aj WITH(NOLOCK)
                                                    INNER JOIN	SALE_Opportunity o WITH(NOLOCK) on o.SaleId = aj.SaleId
                                                    WHERE		aj.Active = 1 AND aj.CancelFlag = 0 AND o.SaleDate IS NOT NULL AND o.SaleDate > @0",
                                            message.MaxSaleDate, Systems.TIPS);

            _healthCheckDatabase.InsertBulk(tipsJobs);
        }
    }

    public class LoadSoldJobsFromTipsRequest : IRequest
    {
        public LoadSoldJobsFromTipsRequest() { }

        public LoadSoldJobsFromTipsRequest(DateTime maxSaleDate)
        {
            MaxSaleDate = maxSaleDate;
        }

        public DateTime MaxSaleDate { get; set; }
    }
}