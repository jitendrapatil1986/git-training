using System;
using NServiceBus;

namespace Warranty.InnerMessages
{
    public class NotifyCommunityWarrantyRepresentativeAssignmentChanged : ICommand
    {
        public Guid CommunityId { get; set; }
        public string CommunityNumber { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeNumber { get; set; }
    }
}