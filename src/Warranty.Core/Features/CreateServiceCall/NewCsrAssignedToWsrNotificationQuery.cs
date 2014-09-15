using System;

namespace Warranty.Core.Features.CreateServiceCall
{
    public class NewCsrAssignedToWsrNotificationQuery : IQuery<NewCsrAssignedToWsrNotificationModel>
    {
        public Guid ServiceCallId { get; set; }
    }
}