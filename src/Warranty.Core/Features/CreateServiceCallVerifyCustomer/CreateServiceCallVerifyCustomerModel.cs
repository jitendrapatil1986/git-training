using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warranty.Core.Features.CreateServiceCallVerifyCustomer
{
    using Entities;

    public class CreateServiceCallVerifyCustomerModel
    {
        public Customer CustomerDetail { get; set; }

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
