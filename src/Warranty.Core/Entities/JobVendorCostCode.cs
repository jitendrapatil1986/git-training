namespace Warranty.Core.Entities
{
    using System;

    public class JobVendorCostCode : IAuditableEntity
    {
        public virtual Guid JobVendorCostCodeId { get; set; }
        public virtual Guid VendorId { get; set; }
        public virtual Guid JobId { get; set; }
        public string CostCode { get; set; }
        public string CostCodeDescription { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
    }
}