namespace Warranty.Core.Features.ServiceCallToggleActions
{
    using System;

    public class ServiceCallCompleteWsrNotificationQuery : IQuery<ServiceCallCompleteWsrNotificationModel>
    {
        public Guid ServiceCallId { get; set; } 
    }
}