namespace Warranty.Core.Entities
{
    using System;

    public class Job : IAuditableEntity, IJdeEntity
    {
        public virtual Guid JobId { get; set; }
        public virtual string JobNumber { get; set; }
        public virtual DateTime? CloseDate { get; set; }
        public virtual string AddressLine { get; set; }
        public virtual string City { get; set; }
        public virtual string StateCode { get; set; }
        public virtual string PostalCode { get; set; }
        public virtual string LegalDescription { get; set; }
        public virtual Guid CommunityId { get; set; }
        public virtual Guid? CurrentHomeOwnerId { get; set; }
        public virtual string PlanType { get; set; }
        public virtual string PlanTypeDescription { get; set; }
        public virtual string PlanName { get; set; }
        public virtual string PlanNumber { get; set; }
        public virtual string Elevation { get; set; }
        public virtual string Swing { get; set; }
        public virtual int Stage { get; set; }
        public virtual Guid? BuilderEmployeeId { get; set; }
        public virtual Guid? SalesConsultantEmployeeId { get; set; }
        public virtual DateTime? WarrantyExpirationDate { get; set; }
        public virtual bool DoNotContact { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
        public string JdeIdentifier { get; set; }

        public bool IsOutOfWarranty
        {
            get
            {
                return CloseDate.GetValueOrDefault().CompareTo(DateTime.Today.AddYears(-2)) <= 0;
            }
        }

        public bool WasCreatedDuringWarrantablePeriod(ServiceCall call)
        {
            return CloseDate >= call.CreatedDate.GetValueOrDefault().AddYears(-2);
        }

        public bool IsNew()
        {
            return JobId == Guid.Empty;
        }
    }
}
