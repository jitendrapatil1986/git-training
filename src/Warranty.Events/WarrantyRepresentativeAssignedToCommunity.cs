using System;
using NServiceBus;

namespace Warranty.Events
{
    public class WarrantyRepresentativeAssignedToCommunity : IEvent
    {
        public Guid CommunityId { get; set; }
        public string CommunityNumber { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeNumber { get; set; }
    }
}