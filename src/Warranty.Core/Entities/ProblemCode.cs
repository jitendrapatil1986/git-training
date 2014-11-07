namespace Warranty.Core.Entities
{
    using System;

    public class ProblemCode : IAuditableEntity
    {
        public virtual int ProblemCodeId { get; set; }
        public virtual string CategoryCode { get; set; }
        public virtual string JdeCode { get; set; }
        public virtual string DetailCode { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
    }
}
