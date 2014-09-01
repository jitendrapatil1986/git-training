using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warranty.Core.Features.JobSummary
{
    public class JobSummaryQuery : IQuery<JobSummaryModel>
    {
        public Guid JobId { get; set; }
    }
}
