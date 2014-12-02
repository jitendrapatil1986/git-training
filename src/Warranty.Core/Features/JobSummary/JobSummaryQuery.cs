using System;

namespace Warranty.Core.Features.JobSummary
{
    public class JobSummaryQuery : IQuery<JobSummaryModel>
    {
        public Guid JobId { get; set; }
    }
}
