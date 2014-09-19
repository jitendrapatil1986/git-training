using System;

namespace Warranty.Core.Features.ServiceCallToggleActions
{
    public class ServiceCallToogleSpecialProjectCommand : ICommand
    {
        public Guid ServiceCallId { get; set; }
        public string Text { get; set; }
    }
}