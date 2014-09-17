using System;
using System.Collections.Generic;

namespace Warranty.Core.ToDoInfrastructure.Models
{
    public class ToDoCommunityEmployeeAssignmentModel
    {
        public Guid CommunityId { get; set; }
        public string CommunityNumber { get; set; }
        public string CommunityName { get; set; }
        public string Market { get; set; }

        public IList<EmployeeViewModel> Employees { get; set; }

        public class EmployeeViewModel
        {
            public string EmployeeNumber { get; set; }
            public string DisplayName { get; set; }
        }
    }
}