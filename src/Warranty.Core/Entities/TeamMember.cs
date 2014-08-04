using System;

namespace Warranty.Core.Entities
{
    public class TeamMember : IAuditableEntity
    {
        public virtual int TeamMemberId { get; set; }
        public virtual string Number { get; set; }
        public virtual string Name { get; set; }
        public virtual string Fax { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
    }
}