using System;

namespace Warranty.Core.Entities
{
    public class CommunityAssignment: IAuditableEntity
    {
        public virtual Guid EmployeeAssignmentId { get; set; }
        public virtual Guid CommunityId { get; set; }
        public virtual Guid EmployeeId { get; set; }
        public DateTime? AssignmentDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}