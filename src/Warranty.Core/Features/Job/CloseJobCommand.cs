using System;

namespace Warranty.Core.Features.Job
{
    public class CloseJobCommand : ICommand
    {
        public CloseJobCommand(Guid jobId, DateTime closeDate)
        {
            JobId = jobId;
            CloseDate = closeDate;
        }

        public Guid JobId { get; set; }
        public DateTime CloseDate { get; set; }
    }
}