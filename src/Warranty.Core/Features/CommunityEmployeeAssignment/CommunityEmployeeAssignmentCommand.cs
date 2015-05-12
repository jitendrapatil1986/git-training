using System;

namespace Warranty.Core.Features.CommunityEmployeeAssignment
{
    public class CommunityEmployeeAssignmentCommand : ICommand<bool>
    {
        public Guid CommunityId { get; set; }
        public string Employeenumber { get; set; }
    }
}
