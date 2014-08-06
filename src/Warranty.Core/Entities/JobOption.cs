﻿using System;

namespace Warranty.Core.Entities
{
    public class JobOption : IAuditableEntity
    {
        public virtual Guid JobOptionId { get; set; }
        public virtual Guid JobId { get; set; }
        public virtual string OptionNumber { get; set; }
        public virtual string OptionDescription { get; set; }
        public virtual int Quantity { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
    }
}