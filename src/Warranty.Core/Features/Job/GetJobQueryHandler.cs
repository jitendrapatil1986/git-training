using Warranty.Core.Services;

namespace Warranty.Core.Features.Job
{
    public class GetJobQueryHandler : IQueryHandler<GetJobQuery, Entities.Job>
    {
        private readonly IJobService _jobService;

        public GetJobQueryHandler(IJobService jobService)
        {
            _jobService = jobService;
        }

        public Entities.Job Handle(GetJobQuery query)
        {
            return _jobService.GetJobByNumber(query.JobNumber);
        }
    }
}