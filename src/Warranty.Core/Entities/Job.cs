namespace Warranty.Core.Entities
{
    using System;

    public class Job : IAuditableEntity
    {
        public virtual int JobId { get; set; }
        public virtual string JobNumber { get; set; }
        public virtual DateTime CloseDate { get; set; }
        public virtual string AddressLine { get; set; }
        public virtual string City { get; set; }
        public virtual string StateCode { get; set; }
        public virtual string PostalCode { get; set; }
        public virtual string LegalDescription { get; set; }
        public virtual int CommunityId { get; set; }
        public virtual int CurrentHomeOwnerId { get; set; }
        public virtual string PlanType { get; set; }
        public virtual string PlanTypeDescription { get; set; }
        public virtual string PlanName { get; set; }
        public virtual string PlanNumber { get; set; }
        public virtual string Elevation { get; set; }
        public virtual string Swing { get; set; }
        public virtual int BuilderEmployeeId { get; set; }
        public virtual int SalesConsultantEmployeeId { get; set; }
        public virtual DateTime WarrantyExpirationDate { get; set; }
        public virtual Decimal TotalPrice { get; set; }
        public virtual bool DoNotContact { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
    }
}