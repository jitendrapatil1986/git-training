using System.Collections.Generic;
using MediatR;
using Warranty.HealthCheck.Data;
using Warranty.HealthCheck.Models;

namespace Warranty.HealthCheck.Mediatr
{
    public class GetShowcasesHandler : IRequestHandler<GetShowcases, IEnumerable<HEALTH_Showcase>>
    {
        private readonly IHealthCheckDatabase _healthCheckDatabase;

        public GetShowcasesHandler(IHealthCheckDatabase healthCheckDatabase)
        {
            _healthCheckDatabase = healthCheckDatabase;
        }

        public IEnumerable<HEALTH_Showcase> Handle(GetShowcases message)
        {
            return _healthCheckDatabase.Fetch<HEALTH_Showcase>("WHERE System = @0", message.System);
        }
    }

    public class GetShowcases : IRequest<IEnumerable<HEALTH_Showcase>>
    {
        public GetShowcases() { }

        public GetShowcases(int system)
        {
            System = system;
        }

        public int System { get; set; }
    }
}