using System.Collections.Generic;
using MediatR;
using Warranty.HealthCheck.Data;

namespace Warranty.HealthCheck.Mediatr
{
    public class GetSoldJobsRequestHandler : IRequestHandler<GetSoldJobsRequest, IEnumerable<string>>
    {
        private readonly IHealthCheckDatabase _healthCheckDatabase;

        public GetSoldJobsRequestHandler(IHealthCheckDatabase healthCheckDatabase)
        {
            _healthCheckDatabase = healthCheckDatabase;
        }

        public IEnumerable<string> Handle(GetSoldJobsRequest message)
        {
            return _healthCheckDatabase.Fetch<string>("SELECT JobNumber FROM HEALTH_SoldJob WHERE System = @0", message.System);
        }
    }

    public class GetSoldJobsRequest : IRequest<IEnumerable<string>>
    {
        public GetSoldJobsRequest()
        {
        }

        public GetSoldJobsRequest(int system)
        {
            System = system;
        }

        public int System { get; set; }
    }
}