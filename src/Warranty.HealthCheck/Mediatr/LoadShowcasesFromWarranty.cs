using MediatR;
using Warranty.HealthCheck.Data;
using Warranty.HealthCheck.Models;

namespace Warranty.HealthCheck.Mediatr
{
    public class LoadShowcasesFromWarrantyHandler : RequestHandler<LoadShowcasesFromWarranty>
    {
        private readonly IWarrantyDatabase _warrantyDatabase;
        private readonly IHealthCheckDatabase _healthCheckDatabase;

        public LoadShowcasesFromWarrantyHandler(IWarrantyDatabase warrantyDatabase, IHealthCheckDatabase healthCheckDatabase)
        {
            _warrantyDatabase = warrantyDatabase;
            _healthCheckDatabase = healthCheckDatabase;
        }

        protected override void HandleCore(LoadShowcasesFromWarranty message)
        {
            var warrantyJobs = _warrantyDatabase.Fetch<HEALTH_Showcase>("SELECT JobNumber, @0 as System FROM Jobs", Systems.Warranty);

            _healthCheckDatabase.InsertBulk(warrantyJobs);
        }
    }

    public class LoadShowcasesFromWarranty : IRequest { }
}