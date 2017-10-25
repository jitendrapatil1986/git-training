using System;
using System.Collections.Generic;
using Warranty.Core.Entities;

namespace Warranty.Core.Features.AssignWSRs
{
    using System.Linq;

    public class AssignWSRsModel
    {
        public AssignWSRsModel()
        {
            Communities = new List<Community>();
        }

        public List<Community> Communities { get; set; }

        public class Community
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public bool IsAssigned { get { return Employees.Any(); } }
            public List<AssignedEmployee> Employees { get; set; }
        }

        public Guid SelectedCommunityId { get; set; }
        public Guid SelectedEmployeeId { get; set; }

    }
}
