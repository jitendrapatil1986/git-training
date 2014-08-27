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
        public Guid HomeOwnerId { get; set; }
        public Guid JobId { get; set; }
        public int HomeOwnerNumber { get; set; }
        public string HomeOwnerName { get; set; }
        public string HomePhone { get; set; }
        public string OtherPhone { get; set; }
        public string WorkPhone1 { get; set; }
        public string WorkPhone2 { get; set; }
        public string EmailAddress { get; set; }
        public string JobNumber { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public string StateCode { get; set; }
        public string PostalCode { get; set; }
        public DateTime WarrantyStartDate { get; set; }
        public int YearsWithinWarranty { get; set; }
    }
}
