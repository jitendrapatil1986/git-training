using System;
using System.Collections.Generic;
using Warranty.Core.Entities;

namespace Warranty.Core.Features.AssignWSRs
{
    public class AssignWSRsModel
    {
        public AssignWSRsModel()
        {
            Communities = new List<Community>();
            Employees = new List<Employee>();
        }

        public List<Community> Communities { get; set; }
        public List<Employee> Employees { get; set; }

        public class Community
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public List<Employee> Employees { get; set; }
        }
    }
}
