namespace Warranty.Core.Features.ServiceCallToggleActions
{
    using System;

    public class ServiceCallCloseCommand : ICommand
    {
        public Guid ServiceCallId { get; set; }
    }
}