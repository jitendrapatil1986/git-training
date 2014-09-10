using System;

namespace Warranty.Core.Features.AssignWSRs
{
    public class AddAssignmentModel
    {
        public Guid CommunityId { get; set; }
        public Guid EmployeeId { get; set; }
    }
}