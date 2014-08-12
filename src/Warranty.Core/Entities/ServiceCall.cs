using Warranty.Core.Enumerations;

namespace Warranty.Core.Entities
{
    using System;

    public class ServiceCall : IAuditableEntity
    {
        public virtual Guid ServiceCallId { get; set; }
        public virtual int ServiceCallNumber { get; set; }
        public virtual string ServiceCallType { get; set; }
        public virtual bool IsSpecialProject { get; set; }
        public virtual ServiceCallStatus ServiceCallStatus { get; set; }
        public virtual bool IsEscalated { get; set; }
        public virtual Guid JobId { get; set; }
        public virtual string Contact { get; set; }
        public virtual Guid WarrantyRepresentativeEmployeeId { get; set; }
        public virtual DateTime CompletionDate { get; set; }
        public virtual string WorkSummary { get; set; }
        public virtual string HomeOwnerSignature { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
    }
}