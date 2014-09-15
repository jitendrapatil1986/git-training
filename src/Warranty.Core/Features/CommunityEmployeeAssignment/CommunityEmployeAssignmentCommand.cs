using System;

namespace Warranty.Core.Features.CommunityEmployeeAssignment
{
    public class CommunityEmployeAssignmentCommand : ICommand<bool>
    {
        public Guid CommunityId { get; set; }
        public string Employeenumber { get; set; }
    }
}
