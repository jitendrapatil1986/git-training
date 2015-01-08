namespace Warranty.Core.Features.ServiceCallToggleActions
{
    using System;
    using System.Collections.Generic;

    public class ServiceCallCompleteWsrNotificationModel
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
        public string Url { get; set; }
        public List<string> Comments { get; set; }
    }
}