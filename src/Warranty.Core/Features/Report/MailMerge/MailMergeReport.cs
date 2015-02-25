namespace Warranty.Core.Features.Report.MailMerge
{
    using System.Collections.Generic;

    public class MailMergeReport
    {
        public IEnumerable<Customer> Customers { get; set; }
        public class Customer
        {
            public string EmptyField1 { get; set; }
            public string EmptyField2 { get; set; }
            public string HomeownerName { get; set; }
            public string AddressLine { get; set; }
            public string City { get; set; }
            public string StateCode { get; set; }
            public string PostalCode { get; set; }
            public string HomePhone { get; set; }
            public string CommunityName { get; set; }
            public string CloseDate { get; set; }
        }
    }
}