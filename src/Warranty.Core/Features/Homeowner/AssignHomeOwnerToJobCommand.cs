using System;

namespace Warranty.Core.Features.Homeowner
{
    public class AssignHomeOwnerToJobCommand : ICommand
    {
        public AssignHomeOwnerToJobCommand(Guid homeOwnerId, Guid jobId)
        {
            HomeOwnerId = homeOwnerId;
            JobId = jobId;
        }

        public Guid HomeOwnerId { get; set; }    
        public Guid JobId { get; set; }
    }
}