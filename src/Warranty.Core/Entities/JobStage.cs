using System;

namespace Warranty.Core.Entities
{
    public class JobStage : IAuditableEntity, IJdeEntity
    {
        public virtual Guid JobId { get; set; }
        public virtual int Stage { get; set; }
        public virtual DateTime? CompletionDate { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
        public virtual string JdeIdentifier { get; set; }
    }
}