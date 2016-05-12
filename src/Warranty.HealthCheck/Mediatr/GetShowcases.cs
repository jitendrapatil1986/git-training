using System.Collections.Generic;
using MediatR;
using Warranty.HealthCheck.Data;

namespace Warranty.HealthCheck.Mediatr
{
    public class GetShowcasesHandler : IRequestHandler<GetShowcases, IEnumerable<string>>
    {
        private readonly IHealthCheckDatabase _healthCheckDatabase;

        public GetShowcasesHandler(IHealthCheckDatabase healthCheckDatabase)
        {
            _healthCheckDatabase = healthCheckDatabase;
        }

        public IEnumerable<string> Handle(GetShowcases message)
        {
            return _healthCheckDatabase.Fetch<string>("SELECT JobNumber FROM HEALTH_Showcase WHERE System = @0", message.System);
        }
    }

    public class GetShowcases : IRequest<IEnumerable<string>>
    {
        public GetShowcases() { }

        public GetShowcases(int system)
        {
            System = system;
        }

        public int System { get; set; }
    }
}