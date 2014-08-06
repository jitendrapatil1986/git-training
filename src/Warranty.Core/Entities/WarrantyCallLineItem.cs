using System;

namespace Warranty.Core.Entities
{
    public class WarrantyCallLineItem : IAuditableEntity
    {
        public virtual int WarrantyCallLineItemId { get; set; }
        public virtual int WarrantyCallId { get; set; }
        public virtual int LineNumber { get; set; }
        public virtual string ProblemCode { get; set; }
        public virtual string ProblemDescription { get; set; }
        public virtual string CauseDescription { get; set; }
        public virtual string ClassificationNote { get; set; }
        public virtual string LineItemRoot { get; set; }
        public virtual bool Completed { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
    }
}

