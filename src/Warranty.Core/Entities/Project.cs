using System;

namespace Warranty.Core.Entities
{
    public class Project : IAuditableEntity
    {
        public virtual Guid ProjectId { get; set; }
        public virtual string ProjectNumber { get; set; }
        public virtual string ProjectName { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
    }
}