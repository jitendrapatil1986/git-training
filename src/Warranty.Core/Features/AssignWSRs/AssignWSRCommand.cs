using System;

namespace Warranty.Core.Features.AssignWSRs
{
    public class AssignWSRCommand : ICommand
    {
        public Guid CommunityId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid EmployeeAssignmentId { get; set; }
    }
}