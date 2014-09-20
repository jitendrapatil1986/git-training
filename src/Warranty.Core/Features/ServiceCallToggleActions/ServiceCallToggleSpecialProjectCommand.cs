using System;

namespace Warranty.Core.Features.ServiceCallToggleActions
{
    public class ServiceCallToggleSpecialProjectCommand : ICommand
    {
        public Guid ServiceCallId { get; set; }
        public string Text { get; set; }
    }
}