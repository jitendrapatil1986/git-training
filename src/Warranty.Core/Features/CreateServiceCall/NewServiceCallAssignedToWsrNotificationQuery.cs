using System;

namespace Warranty.Core.Features.CreateServiceCall
{
    public class NewServiceCallAssignedToWsrNotificationQuery : IQuery<NewServiceCallAssignedToWsrNotificationModel>
    {
        public Guid ServiceCallId { get; set; }
    }
}