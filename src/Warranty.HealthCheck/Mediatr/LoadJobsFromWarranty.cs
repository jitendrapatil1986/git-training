using MediatR;
using Warranty.HealthCheck.Data;
using Warranty.HealthCheck.Models;

namespace Warranty.HealthCheck.Mediatr
{
    public class LoadJobsFromWarrantyRequestHandler : RequestHandler<LoadJobsFromWarrantyRequest>
    {
        private readonly IWarrantyDatabase _warrantyDatabase;
        private readonly IHealthCheckDatabase _healthCheckDatabase;

        public LoadJobsFromWarrantyRequestHandler(IWarrantyDatabase warrantyDatabase, IHealthCheckDatabase healthCheckDatabase)
        {
            _warrantyDatabase = warrantyDatabase;
            _healthCheckDatabase = healthCheckDatabase;
        }

        protected override void HandleCore(LoadJobsFromWarrantyRequest message)
        {
            var warrantyJobs = _warrantyDatabase.Fetch<HEALTH_SoldJob>("SELECT JobNumber, @0 as System FROM Jobs", Systems.Warranty);

            _healthCheckDatabase.InsertBulk(warrantyJobs);
        }
    }

    public class LoadJobsFromWarrantyRequest : IRequest { }
}