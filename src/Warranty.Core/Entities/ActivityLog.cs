using System;
using Warranty.Core.Enumerations;

namespace Warranty.Core.Entities
{
    public class ActivityLog : IAuditCreatedEntity
    {
        public virtual Guid ActivityLogId { get; set; }
        public virtual string ActivityName { get; set; }
        public virtual string Details { get; set; }
        public virtual Guid ReferenceId { get; set; }
        public virtual ReferenceType ReferenceType { get; set; }
        public virtual ActivityType ActivityType { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime CreatedDate { get; set; }
    }
}