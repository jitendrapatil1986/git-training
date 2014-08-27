namespace Warranty.Core.Features.CreateServiceCallCustomerSearch
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Entities;

    public class CreateServiceCallCustomerSearchModel
    {
        public IEnumerable<Customer> Customers { get; set; }

        public class Customer : HomeOwner
        {
            public string JobNumber { get; set; }
            public string Address1 { get; set; }
            public string City { get; set; }
            public string StateCode { get; set; }
            public string PostalCode { get; set; }
            public DateTime WarrantyStartDate { get; set; }
            public int YearsWithinWarranty { get; set; }
        }
    }
}
