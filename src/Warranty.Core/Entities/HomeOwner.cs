﻿namespace Warranty.Core.Entities
{
    using System;

    public class HomeOwner : IAuditableEntity
    {
        public virtual Guid HomeOwnerId { get; set; }
        public virtual Guid? JobId { get; set; }
        public virtual int? HomeOwnerNumber { get; set; }
        public virtual string HomeOwnerName { get; set; }
        public virtual string HomePhone { get; set; }
        public virtual string OtherPhone { get; set; }
        public virtual string WorkPhone1 { get; set; }
        public virtual string WorkPhone2 { get; set; }
        public virtual string EmailAddress { get; set; }
        
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
    }
}