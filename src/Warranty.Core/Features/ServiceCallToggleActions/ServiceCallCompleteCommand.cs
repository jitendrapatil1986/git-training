namespace Warranty.Core.Features.ServiceCallToggleActions
{
    using System;

    public class ServiceCallCompleteCommand : ICommand
    {
        public Guid ServiceCallId { get; set; }
    }
}