using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warranty.Core.Features.CreateServiceCall
{
    using System.Web.Mvc;
    using Entities;
    using Enumerations;

    public class CreateServiceCallModel
    {
        public IEnumerable<SelectListItem> ProblemCodeList { get; set; }

        public Guid ServiceCallId { get; set; }
        public int ServiceCallNumber { get; set; }
        public string ServiceCallType { get; set; }
        public bool IsSpecialProject { get; set; }
        public ServiceCallStatus ServiceCallStatus { get; set; }
        public string Contact { get; set; }
        public Guid WarrantyRepresentativeEmployeeId { get; set; }
        public DateTime CompletionDate { get; set; }
        public string WorkSummary { get; set; }
        public string HomeOwnerSignature { get; set; }

        public Guid HomeOwnerId { get; set; }
        public Guid JobId { get; set; }
        public string JobNumber { get; set; }
        public int HomeOwnerNumber { get; set; }
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

        public IEnumerable<ServiceCallComment> ServiceCallHeaderComments { get; set; }
        public IEnumerable<ServiceCallLineItemModel> ServiceCallLineItems { get; set; }
        //public IEnumerable<ServiceCallLineItemComments> CallLineItemComments { get; set; }

        
        public class ServiceCallLineItemModel
        {
            public string ProblemCode { get; set; }
            public string ProblemDescription { get; set; }
        }
        
    }
}
