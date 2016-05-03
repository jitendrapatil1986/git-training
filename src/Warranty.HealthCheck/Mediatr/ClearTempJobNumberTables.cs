using MediatR;
using Warranty.HealthCheck.Data;

namespace Warranty.HealthCheck.Mediatr
{
    public class ClearTempJobNumberTablesRequestHandler : RequestHandler<ClearTempJobNumberTablesRequest>
    {
        private readonly IHealthCheckDatabase _healthCheckDatabase;

        public ClearTempJobNumberTablesRequestHandler(IHealthCheckDatabase healthCheckDatabase)
        {
            _healthCheckDatabase = healthCheckDatabase;
        }

        protected override void HandleCore(ClearTempJobNumberTablesRequest message)
        {
            _healthCheckDatabase.Execute("DELETE FROM HEALTH_SoldJob");
        }
    }

    public class ClearTempJobNumberTablesRequest : IRequest
    {
        
    }
}