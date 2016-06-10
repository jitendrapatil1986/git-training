using MediatR;
using Warranty.HealthCheck.Data;

namespace Warranty.HealthCheck.Mediatr
{
    public class ClearTempShowcasesTableHandler : RequestHandler<ClearTempShowcasesTable>
    {
        private readonly IHealthCheckDatabase _healthCheckDatabase;

        public ClearTempShowcasesTableHandler(IHealthCheckDatabase healthCheckDatabase)
        {
            _healthCheckDatabase = healthCheckDatabase;
        }

        protected override void HandleCore(ClearTempShowcasesTable message)
        {
            _healthCheckDatabase.Execute("DELETE FROM HEALTH_Showcase");
        }
    }

    public class ClearTempShowcasesTable : IRequest { }
}