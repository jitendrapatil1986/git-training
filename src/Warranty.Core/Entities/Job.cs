namespace Warranty.Core.Entities
{
    using System;
    using Configurations;
    using Yay.Enumerations;

    public class Job : IAuditableEntity
    {
        public Guid Id { get; set; }
        public string JobNumber { get; set; }
        public string LegalDescription { get; set; }
        public Community Community { get; set; }
        public HomeOwner CurrentHomeowner { get; set; }

        public Address PhysicalAddress { get; set; }

        public Plan Plan { get; set; }
        public string Elevation { get; set; }
        public string Swing { get; set; }

        public Employee Builder { get; set; }
        public Employee SalesConsultant { get; set; }
        public Decimal TotalPrice { get; set; }

        public DateTime WarrantyExpirationDate { get { return CloseDate.AddYears(WarrantyConstants.NumberOfYearsHomeIsWarrantable); }}
        public DateTime CloseDate { get; set; }
        
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }

    public class Address
    {
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string StateCode { get; set; }
        public string PostalCode { get; set; }

    }

    public class Plan
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public PlanType Type { get; set; }
    }

    public class PlanType : Enumeration<PlanType>
    {
        public PlanType(int value, string displayName) : base(value, displayName)
        {
        }
    }
}