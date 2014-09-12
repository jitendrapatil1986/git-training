namespace Warranty.Core.Features.CreateServiceCall
{
    using System;
    using System.Collections.Generic;

    public class CreateServiceCallModel
    {
        public CreateServiceCallModel()
        {
            ServiceCallLineItems = new List<ServiceCallLineItemForm>();
        }

        public string JobNumber { get; set; }
        public string HomeOwnerName { get; set; }
        public string HomePhone { get; set; }
        public string OtherPhone { get; set; }
        public string WorkPhone1 { get; set; }
        public string WorkPhone2 { get; set; }
        public string EmailAddress { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public string StateCode { get; set; }
        public string PostalCode { get; set; }
        public string SalesPersonName { get; set; }
        public string BuilderName { get; set; }
        public DateTime WarrantyStartDate { get; set; }
        public int YearsWithinWarranty { get; set; }

        public Guid JobId { get; set; }
        public string Contact { get; set; }
        public IEnumerable<ServiceCallLineItemForm> ServiceCallLineItems { get; set; }

        public class ServiceCallLineItemForm
        {
            public string ProblemCodeId { get; set; }
            public string ProblemCodeDisplayName { get; set; }
            public string ProblemDescription { get; set; }
            public int LineItemNumber { get; set; }
        }
    }
}
