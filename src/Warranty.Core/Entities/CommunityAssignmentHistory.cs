using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warranty.Core.Entities
{
    public class CommunityAssignmentHistory : IAuditableEntity
    {
        public virtual Guid EmployeeAssignmentHistoryId { get; set; }
        //public virtual Guid EmployeeAssignmentId { get; set; }
        public virtual Guid CommunityId { get; set; }
        public virtual Guid EmployeeId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime AssignmentDate { get; set; }
    }
}
