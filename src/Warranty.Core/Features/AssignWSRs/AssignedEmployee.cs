using System;
using Warranty.Core.Entities;

namespace Warranty.Core.Features.AssignWSRs
{
    public class AssignedEmployee : Employee
    {
        public Guid EmployeeAssignmentId { get; set; }
    }
}