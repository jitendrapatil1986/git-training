using System.Collections;
using System.Collections.Generic;
using MediatR;
using Warranty.HealthCheck.Data;

namespace Warranty.HealthCheck.Mediatr
{
    public class GetClosedJobsRequest : IRequest<IEnumerable<string>>
    {
        public int System { get; set; }

        public GetClosedJobsRequest()
        {
        }

        public GetClosedJobsRequest(int system)
        {
            System = system;
        }
    }

    public class GetClosedJobsRequestHandler : IRequestHandler<GetSoldJobsRequest, IEnumerable<string>>
    {
        private readonly IHealthCheckDatabase _healthCheckDatabase;

        public GetClosedJobsRequestHandler(IHealthCheckDatabase healthCheckDatabase)
        {
            _healthCheckDatabase = healthCheckDatabase;
        }

        public IEnumerable<string> Handle(GetSoldJobsRequest message)
        {
            return _healthCheckDatabase
                .Fetch<string>("SELECT JobNumber FROM dbo.HEALTH_MissingJobs WHERE System = @0;", message.System);
        }
    }
}