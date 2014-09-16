using System;

namespace Warranty.Core.Entities
{
    public class CommunityAssignment
    {
        public virtual Guid EmployeeAssignmentId { get; set; }
        public virtual Guid CommunityId { get; set; }
        public virtual Guid EmployeeId { get; set; }
    }
}