namespace Warranty.Core.Entities
{
    using System;
    using Enumerations;

    public class ServiceCallLineItem : IAuditableEntity
    {
        public virtual Guid ServiceCallLineItemId { get; set; }
        public virtual Guid ServiceCallId { get; set; }
        public virtual int LineNumber { get; set; }
        public virtual string ProblemCode { get; set; }
        public virtual string ProblemDescription { get; set; }
        public virtual string CauseDescription { get; set; }
        public virtual string ClassificationNote { get; set; }
        public virtual string LineItemRoot { get; set; }
        public virtual ServiceCallLineItemStatus ServiceCallLineItemStatus { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
        public virtual string RootCause { get; set; }
        public virtual string RootProblem { get; set; }
        public virtual string ProblemJdeCode { get; set; }
    }
}

