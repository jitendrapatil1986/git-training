using System;
using System.Collections.Generic;

namespace Warranty.Core.Features.CreateServiceCall
{
    public class NewServiceCallAssignedToWsrNotificationModel
    {
        public Guid ServiceCallId { get; set; }
        public string ServiceCallNumber { get; set; }
        public Guid WarrantyRepresentativeEmployeeId { get; set; }
        public string WarrantyRepresentativeEmployeeEmail { get; set; }
        public string HomeOwnerName { get; set; }
        public string HomePhone { get; set; }
        public string CommunityName { get; set; }
        public string AddressLine { get; set; }
        public string EmployeeNumber { get; set; }
        public List<string> Comments { get; set; }    
        public List<ServiceCallLine> LineItems { get; set; }        
        public string Url { get; set; }
    }

    public class ServiceCallLine
    {
        public int LineNumber { get; set; }
        public string ProblemCode { get; set; }    
        public string ProblemDescription { get; set; }                  
    }
   
}
