namespace Warranty.Core.Entities
{
    using System;
    using Enumerations;

    public class Task : IAuditableEntity
    {
        public virtual Guid TaskId { get; set; }
        public virtual Guid EmployeeId { get; set; }
        public virtual Guid ReferenceId { get; set; }
        public virtual string Description { get; set; }
        public virtual bool IsComplete { get; set; }
        public virtual bool IsNoAction { get; set; }
        public virtual TaskType TaskType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}