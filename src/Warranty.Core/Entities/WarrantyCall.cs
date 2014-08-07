namespace Warranty.Core.Entities
{
    using System;

    public class WarrantyCall : IAuditableEntity
    {
        public virtual int WarrantyCallId { get; set; }
        public virtual string WarrantyCallNumber { get; set; }
        public virtual string WarrantyCallType { get; set; }
        public virtual int JobId { get; set; }
        public virtual string Contact { get; set; }
        public virtual int WarrantyRepresentativeEmployeeId { get; set; }
        public virtual DateTime CompletionDate { get; set; }
        public virtual string WorkSummary { get; set; }
        public virtual string HomeOwnerSignature { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
    }
}