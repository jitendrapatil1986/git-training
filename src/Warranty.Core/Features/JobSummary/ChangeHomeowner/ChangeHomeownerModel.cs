namespace Warranty.Core.Features.JobSummary.ChangeHomeowner
{
    using System;

    public class ChangeHomeownerModel
    {
        public Guid JobId { get; set; }
        public CurrentHomeownerDetail CurrentHomeowner { get; set; }

        public class CurrentHomeownerDetail
        {
            public string HomeownerName { get; set; }
            public string HomePhone { get; set; }
            public string OtherPhone { get; set; }
            public string WorkNumber { get; set; }
            public string EmailAddress { get; set; }
            public int HomeownerNumber { get; set; }
        }

        public string JobNumber { get; set; }
        public DateTime? CloseDate { get; set; }
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string StateCode { get; set; }
        public string PostalCode { get; set; }
        public int YearsWithinWarranty { get; set; }
        public DateTime WarrantyStartDate { get; set; }
        
        public int? NewHomeownerNumber { get; set; }
        public string NewHomeownerName { get; set; }
        public string NewHomeownerHomePhone { get; set; }
        public string NewHomeownerOtherPhone { get; set; }
        public string NewHomeownerWorkPhone1 { get; set; }
        public string NewHomeownerWorkPhone2 { get; set; }
        public string NewHomeownerEmailAddress { get; set; }
    }
}