using MediatR;
using Warranty.HealthCheck.Data;

namespace Warranty.HealthCheck.Mediatr
{
    public class ClearHomeOwnerMissingInfoTablesRequest : IRequest
    {
    }

    public class ClearHomeOwnerMissingInfoTablesHandler : RequestHandler<ClearHomeOwnerMissingInfoTablesRequest>
    {
        private readonly IHealthCheckDatabase _healthCheckDatabase;

        public ClearHomeOwnerMissingInfoTablesHandler(IHealthCheckDatabase healthCheckDatabase)
        {
            _healthCheckDatabase = healthCheckDatabase;
        }

        protected override void HandleCore(ClearHomeOwnerMissingInfoTablesRequest message)
        {
            _healthCheckDatabase.Execute("DELETE FROM dbo.HEALTH_MissingJobs;");
        }
    }
}