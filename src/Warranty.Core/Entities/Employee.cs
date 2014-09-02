namespace Warranty.Core.Entities
{
    using System;

    public class Employee : IAuditableEntity, IJdeEntity
    {
        public virtual Guid EmployeeId { get; set; }
        public virtual string Number { get; set; }
        public virtual string Name { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
        public virtual string JdeIdentifier { get; set; }
    }
}