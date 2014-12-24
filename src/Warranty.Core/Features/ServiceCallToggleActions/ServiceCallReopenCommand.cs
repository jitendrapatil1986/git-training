namespace Warranty.Core.Features.ServiceCallToggleActions
{
    using System;

    public class ServiceCallReopenCommand : ICommand
    {
        public Guid ServiceCallId { get; set; }
        public string Text { get; set; }
    }
}